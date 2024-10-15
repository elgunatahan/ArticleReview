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
using System.Reflection;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
