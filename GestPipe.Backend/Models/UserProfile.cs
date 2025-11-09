using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestPipe.Backend.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("full_name")]
        public string? FullName { get; set; }

        [BsonElement("profile_image")]
        public string? ProfileImage { get; set; }

        [BsonElement("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("occupation")]
        public string? Occupation { get; set; }

        [BsonElement("company")]
        public string? Company { get; set; }

        [BsonElement("birth_date")]
        public DateTime? BirthDate { get; set; }

        [BsonElement("education_level")]
        public string? EducationLevel { get; set; }

        
        [BsonElement("phone_number")]
        public string? PhoneNumber { get; set; }

        [BsonElement("gender")]
        public string? Gender { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }
    }
}