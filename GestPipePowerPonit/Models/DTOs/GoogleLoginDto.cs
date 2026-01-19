using System.ComponentModel.DataAnnotations;

namespace GestPipePowerPonit.Models.DTOs
{
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "IdToken is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}