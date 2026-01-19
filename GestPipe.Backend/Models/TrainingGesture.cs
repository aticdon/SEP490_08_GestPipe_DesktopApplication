using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestPipe.Backend.Models
{
    public class TrainingGesture
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // nullable nếu muốn
        //public string Id { get; set; }
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("pose_label")]
        public string PoseLabel { get; set; }
        [BsonElement("total_train")]
        public int TotalTrain { get; set; }
        [BsonElement("correct_train")]
        public int CorrectTrain { get; set; }
        [BsonElement("accuracy")]
        public double Accuracy { get; set; }
        [BsonElement("vector_data")]
        public VectorData VectorData { get; set; }
        [BsonElement("create_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateAt { get; set; }
    }
}
