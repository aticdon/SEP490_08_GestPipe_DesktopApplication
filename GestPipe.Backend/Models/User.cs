using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestPipe.Backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("account_status")]
        public string AccountStatus { get; set; } = string.Empty; // "inactive","activeonline","activeoffline","pending","blocked"

        [BsonElement("gesture_request_status")]
        public string GestureRequestStatus { get; set; } = string.Empty;

        [BsonElement("ui_language")]
        public string? UiLanguage { get; set; }

        [BsonElement("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;

        [BsonElement("request_count_today")]
        public int RequestCountToday { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("last_login")]
        public DateTime? LastLogin { get; set; }

        [BsonElement("version_gesture_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? VersionGestureId { get; set; }

        // Social login fields
        [BsonElement("auth_provider")]
        public string? AuthProvider { get; set; } // e.g. "Google"

        [BsonElement("provider_id")]
        public string? ProviderId { get; set; } // e.g. Google 'sub'

        [BsonElement("email_verified")]
        public bool EmailVerified { get; set; } = false;
    }
}