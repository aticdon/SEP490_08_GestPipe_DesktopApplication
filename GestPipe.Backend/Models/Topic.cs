using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestPipe.Backend.Models
{
    public class Topic
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("category_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("difficulty")]
        public string Difficulty { get; set; }
    }
}