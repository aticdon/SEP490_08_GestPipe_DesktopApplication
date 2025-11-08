using System;

namespace GestPipePowerPonit.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Status { get; set; }
        public string StatusGesture { get; set; }
        public string Model { get; set; }
        public int RequestCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string CurrentVer { get; set; }
        public string Language { get; set; }
    }
}