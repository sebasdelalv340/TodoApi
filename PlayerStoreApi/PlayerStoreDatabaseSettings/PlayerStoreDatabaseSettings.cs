namespace PlayerStoreApi.Models;

public class PlayerStoreDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string PlayersCollectionName { get; set; } = null!;
}