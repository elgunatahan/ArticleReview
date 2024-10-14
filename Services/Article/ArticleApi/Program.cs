using Application.Commands.Article.Create;
using Application.Common.Behaviors;
using Application.Common.CurrentUser;
using ArticleApi.Common;
using ArticleApi.Middlewares;
using Domain.Dtos;
using Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MongoDB.Driver;
using Persistence;
using Persistence.Repositories;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;
using System.Reflection;
using Serilog.Exceptions;

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
    options.InstanceName = "ArticleApi_";
});

var mongoUrlBuilder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
//BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
//BsonSerializer.RegisterSerializer(typeof(IEdmModel), new EdmModelSerializer());

MongoDbPersistence.Configure();

new MongoBootstrapper(mongoClient).Migrate();

builder.Services.AddSingleton<ErrorCodeMaps>(x => builder.Configuration.GetSection("ErrorCodeMaps").Get<ErrorCodeMaps>());

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddSingleton<IMongoClient>(x => mongoClient);

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateArticleCommand)));
    cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateArticleCommand)));

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

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();


static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<ArticleDto>("Article");

    return builder.GetEdmModel();
}