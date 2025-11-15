using GestPipe.Backend.Models.DTOs.Auth;

namespace GestPipe.Backend.Models.DTOs.ProfileUser
{
    public class ProfileDataDto
    {
        public UserProfileDto Profile { get; set; } = null!;
        public UserResponseDto User { get; set; } = null!;
    }
}