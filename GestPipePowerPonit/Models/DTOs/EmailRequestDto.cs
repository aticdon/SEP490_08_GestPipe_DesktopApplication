using System.ComponentModel.DataAnnotations;

namespace GestPipePowerPonit.Models.DTOs
{
    public class EmailRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}