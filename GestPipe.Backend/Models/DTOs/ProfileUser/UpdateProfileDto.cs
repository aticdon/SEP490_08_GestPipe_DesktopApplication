using System;
using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Models.DTOs.ProfileUser
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string EducationLevel { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [StringLength(100)]
        public string Occupation { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string AvatarUrl { get; set; }
    }
}