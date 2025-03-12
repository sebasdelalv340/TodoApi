using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Controllers;
using TodoApi.Controllers.Models;


var builder = WebApplication.CreateBuilder(args);

// Leer las variables de entorno
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DB_NAME");

// Validar que las variables de entorno no estén vacías
if (string.IsNullOrEmpty(mongoConnectionString) || string.IsNullOrEmpty(mongoDatabaseName))
{
    throw new InvalidOperationException("MongoDB connection string or database name is missing.");
}

// Configurar MongoDbSettings con las variables de entorno
builder.Services.Configure<MongoDbSettings>(options =>
{
    // Usar el constructor para inicializar el objeto
    options = new MongoDbSettings(mongoConnectionString, mongoDatabaseName);
});

// Registrar el cliente de MongoDB y la base de datos
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Registrar la colección de Players
builder.Services.AddSingleton<IMongoCollection<Player>>(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<Player>("players");
});

// Configuración de los servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Usar el puerto asignado por Render o predeterminado
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://*:{port}");

