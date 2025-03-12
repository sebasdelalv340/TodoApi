namespace TodoApi.Controllers;

public record MongoDbSettings
{
    public string ConnectionString { get; init; }
    public string DatabaseName { get; init; }

    // Constructor personalizado para inicializar las propiedades
    public MongoDbSettings(string connectionString, string databaseName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }
}



