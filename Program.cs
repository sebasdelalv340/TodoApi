using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de MongoDB usando variables de entorno
var connectionString = builder.Configuration["MongoDBSettings:ConnectionString"];
var databaseName = builder.Configuration["MongoDBSettings:DatabaseName"];

var client = new MongoClient(connectionString);
var database = client.GetDatabase(databaseName);

// Registrar la colección de Players
builder.Services.AddSingleton<IMongoCollection<Player>>(database.GetCollection<Player>("players"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Usar el puerto asignado por Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://*:{port}");