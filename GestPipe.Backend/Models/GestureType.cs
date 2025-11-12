using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class GestureType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("type_name")]
    public Dictionary<string, string> TypeName { get; set; } // {"en": "...", "vi": "..."}
    
    [BsonElement("code")]
    public Dictionary<string, string> Code { get; set; } 
}