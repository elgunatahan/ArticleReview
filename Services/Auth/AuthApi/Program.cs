using AuthApi.Common;
using AuthApi.Middlewares;
using AuthApi.Models.Validators;
using AuthApi.Repositories;
using AuthApi.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
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


var mongoUrlBuilder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

MongoDbPersistence.Configure();

new MongoBootstrapper(mongoClient).Migrate();

builder.Services.AddSingleton<RsaSecurityKey>(provider =>
{
    var rsa = RSA.Create();
    var privateKeyBase64 = Environment.GetEnvironmentVariable("JwtAK");
    if (string.IsNullOrEmpty(privateKeyBase64))
    {
        throw new InvalidOperationException("JwtAK is not configured.");
    }
    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
    return new RsaSecurityKey(rsa);
});


builder.Services.AddSingleton<IMongoClient>(x => mongoClient);

builder.Services.AddSingleton<ErrorCodeMaps>(x => builder.Configuration.GetSection("ErrorCodeMaps").Get<ErrorCodeMaps>());

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(LoginRequestValidator)));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
