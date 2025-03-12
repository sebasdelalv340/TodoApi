using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlayerStoreApi.Models;

public class Player
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    public decimal MaxScore { get; set; }
}