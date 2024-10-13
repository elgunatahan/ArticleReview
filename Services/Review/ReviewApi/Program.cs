using Domain.Interfaces.Proxies;
using Proxies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();

builder.Services.AddHttpClient<IArticleApiProxy, ArticleApiProxy>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ARTICLE_API_BASE_URL"));
});

builder.Services.AddControllers();
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

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
