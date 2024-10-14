using Application.Commands.Review.Create;
using Application.Common.Behaviors;
using Application.Common.CurrentUser;
using Domain.Dtos;
using Domain.Interfaces.Proxies;
using Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MongoDB.Driver;
using Persistence;
using Persistence.Repositories;
using Proxies;
using ReviewApi.Common;
using ReviewApi.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var minimumLevelStr = builder.Configuration["Logging:LogLevel:Default"];
if (!Enum.TryParse(minimumLevelStr, out LogLevel minimumLevel))
{
    minimumLevel = LogLevel.Debug;
}

var minimumSerilogLogLevel = (LogEventLevel)minimumLevel;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", minimumSerilogLogLevel)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", minimumSerilogLogLevel)
    .MinimumLevel.Override("System.Net.Http.HttpClient", minimumSerilogLogLevel)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "ReviewApi_";
});

var mongoUrlBuilder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

MongoDbPersistence.Configure();

new MongoBootstrapper(mongoClient).Migrate();

builder.Services.AddSingleton<ErrorCodeMaps>(x => builder.Configuration.GetSection("ErrorCodeMaps").Get<ErrorCodeMaps>());

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddSingleton<IMongoClient>(x => mongoClient);

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateReviewCommand)));
    cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateReviewCommand)));


builder.Services.AddHttpClient();

builder.Services.AddHttpClient<IArticleApiProxy, ArticleApiProxy>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ARTICLE_API_BASE_URL"));
});

//builder.Services.AddControllers();
builder.Services.AddControllers().AddOData(opt => opt
    .Select()
    .Filter()
    .OrderBy()
    .Expand()
    .Count()
    .SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel())
    );

builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

 if (app.Environment.IsDevelopment())
 {
     app.UseDeveloperExceptionPage();
 }
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<ReviewDto>("Review");

    return builder.GetEdmModel();
}