using Application.Commands.Article.Create;
using Application.Common.Behaviors;
using Application.Common.CurrentUser;
using ArticleApi.Common;
using ArticleApi.Middlewares;
using Domain.Dtos;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Persistence;
using Persistence.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var mongoUrlBuilder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(typeof(IEdmModel), new EdmModelSerializer());

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
    // Modelinize entity'leri ekleyin
    //builder.EntitySet<ArticleDto>("Article");
    builder.EntitySet<ArticleDto>("Article")
       .EntityType.Select();
    return builder.GetEdmModel();
}