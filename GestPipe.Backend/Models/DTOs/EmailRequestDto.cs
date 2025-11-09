using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Models.DTOs.Auth
{
    public class EmailRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}