using System;
using System.ComponentModel.DataAnnotations;

namespace GestPipePowerPonit.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ReEnterPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [RegularExpression(@"^\d{10,11}$")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public string? EducationLevel { get; set; }

        public string? Company { get; set; }

        public string? Occupation { get; set; }
    }
}