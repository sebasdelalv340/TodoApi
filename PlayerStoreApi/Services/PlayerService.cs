using PlayerStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PlayerStoreApi.Services;

public class PlayerService
{
    private readonly IMongoCollection<Player> _PlayersCollection;

    public PlayerService(
        IOptions<PlayerStoreDatabaseSettings> PlayerStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            PlayerStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            PlayerStoreDatabaseSettings.Value.DatabaseName);

        _PlayersCollection = mongoDatabase.GetCollection<Player>(
            PlayerStoreDatabaseSettings.Value.PlayersCollectionName);
    }

    public async Task<List<Player>> GetAsync() =>
        await _PlayersCollection.Find(_ => true).ToListAsync();

    public async Task<Player?> GetAsync(string id) =>
        await _PlayersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Player newPlayer) =>
        await _PlayersCollection.InsertOneAsync(newPlayer);

    public async Task UpdateAsync(string id, Player updatedPlayer) =>
        await _PlayersCollection.ReplaceOneAsync(x => x.Id == id, updatedPlayer);

    public async Task RemoveAsync(string id) =>
        await _PlayersCollection.DeleteOneAsync(x => x.Id == id);
}