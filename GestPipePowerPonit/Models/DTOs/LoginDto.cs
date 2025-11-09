using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}