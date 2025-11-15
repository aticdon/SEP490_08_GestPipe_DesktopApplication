using GestPipePowerPonit.Models.DTOs;

namespace GestPipePowerPonit.Models.DTOs
{
    public class ProfileDataDto
    {
        public UserProfileDto Profile { get; set; } = null!;
        public UserResponseDto User { get; set; } = null!;
    }
}