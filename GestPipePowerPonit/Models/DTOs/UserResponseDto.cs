namespace GestPipePowerPonit.Models.DTOs
{
    public class UserResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FullName { get; set; }
        public string? AuthProvider { get; set; }
    }
}
