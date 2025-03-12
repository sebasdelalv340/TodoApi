using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Controllers;

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
    options.ConnectionString = mongoConnectionString;
    options.DatabaseName = mongoDatabaseName;
});

// Registrar IMongoClient y IMongoDatabase
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
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
app.UseCors("AllowGodot");

// Usar el puerto asignado por Render o predeterminado
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");

