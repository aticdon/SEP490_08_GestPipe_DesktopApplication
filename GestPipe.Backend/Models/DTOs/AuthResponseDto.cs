namespace GestPipe.Backend.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool RequiresVerification { get; set; } = false;
        public dynamic Data { get; set; }
       
    }
}