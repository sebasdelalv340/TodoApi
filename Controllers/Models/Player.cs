using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApi.Controllers.Models;

public class Player
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("Name")]
    [JsonPropertyName("Name")]
    public string? Name { get; set; }
    [BsonElement("MaxScore")]
    [JsonPropertyName("MaxScore")]
    public int MaxScore { get; set; }
}