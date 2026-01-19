namespace GestPipePowerPonit.Models.DTOs
{
    public class ProfileResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ProfileDataDto? Data { get; set; }
    }
}