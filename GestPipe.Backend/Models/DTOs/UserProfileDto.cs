namespace GestPipe.Backend.Models.DTOs.Auth
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Occupation { get; set; }
        public string? Company { get; set; }
        public string? EducationLevel { get; set; }
    }
}
