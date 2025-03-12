namespace TodoApi.Controllers;

public record MongoDbSettings
{
    public string ConnectionString { get; set; }  // Cambia init por set
    public string DatabaseName { get; set; }     // Cambia init por set
    
    // Constructor personalizado para inicializar las propiedades
    public MongoDbSettings(string connectionString, string databaseName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }
}




