using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Models
{
    public class Otp
    {
        [BsonId] // ✅ Đánh dấu Email là _id trong MongoDB
        [BsonElement("_id")]
        [Key]
        [MaxLength(256)]
        public string Email { get; set; }

        [BsonElement("otpCode")]
        [Required]
        [MaxLength(10)]
        public string OtpCode { get; set; }

        [BsonElement("purpose")]
        [Required]
        [MaxLength(50)]
        public string Purpose { get; set; }

        [BsonElement("createdAt")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [BsonElement("expiresAt")]
        [Required]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("attemptsCount")]
        public int AttemptsCount { get; set; } = 0;

        
        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    }
}
