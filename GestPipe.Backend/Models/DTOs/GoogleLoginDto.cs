using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Models.DTOs.Auth
{
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "IdToken is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}