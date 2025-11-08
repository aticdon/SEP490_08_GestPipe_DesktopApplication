using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class GestureType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("type_name")]
    public string TypeName { get; set; }

    [BsonElement("code")]
    public string Code { get; set; }
}