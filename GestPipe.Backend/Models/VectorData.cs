using MongoDB.Bson.Serialization.Attributes;

namespace GestPipe.Backend.Models
{
    public class VectorData
    {
            [BsonElement("fingers")]
            public int[] Fingers { get; set; }

            [BsonElement("main_axis_x")]
            public double MainAxisX { get; set; }

            [BsonElement("main_axis_y")]
            public double MainAxisY { get; set; }

            [BsonElement("delta_x")]
            public double DeltaX { get; set; }

            [BsonElement("delta_y")]
            public double DeltaY { get; set; }       
    }
}
