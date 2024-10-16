using Application.Commands.Article.Create;
using Application.Common.Behaviors;
using Application.Common.CurrentUser;
using ArticleApi.Common;
using ArticleApi.Middlewares;
using Domain.Dtos;
using Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Persistence;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System.Reflection;
using System.Security.Cryptography;

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

var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "ArticleApi_";
});

var mongoUrlBuilder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

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


if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8080); // Use HTTP on port 5000
    });
}
else
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8081, listenOptions =>
        {
            listenOptions.UseHttps(); // Use HTTPS in non-development environments
        });
    });
}


builder.Services.AddControllers().AddOData(opt => opt
    .Select()
    .Filter()
    .OrderBy()
    .Expand()
    .Count()
    .SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel())
    );

var publicKey = Environment.GetEnvironmentVariable("JwtVK");
if (string.IsNullOrEmpty(publicKey))
{
    throw new InvalidOperationException("JwtVK not found in environment variables.");
}
using var rsa = RSA.Create();
rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JwtIssuer"),
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Article API",
        Version = "v1",
        Description = "JWT Authorization for Article API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection(); // Sadece production'da HTTPS'ye yönlendir
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<ArticleDto>("Article");

    return builder.GetEdmModel();
}