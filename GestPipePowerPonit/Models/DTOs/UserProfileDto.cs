using System;
using System.ComponentModel.DataAnnotations;
namespace GestPipePowerPonit.Models.DTOs
{
    public class UserProfileDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must be exactly 10 digits and start with 0")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "Education level cannot exceed 100 characters")]
        public string EducationLevel { get; set; }

        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string Company { get; set; }

        [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
        public string Occupation { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string AvatarUrl { get; set; }
    }
}
