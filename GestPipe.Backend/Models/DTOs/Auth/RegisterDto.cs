//using System;
//using System.ComponentModel.DataAnnotations;

//namespace GestPipe.Backend.Models.DTOs.Auth
//{
//    public class RegisterDto
//    {
//        [Required(ErrorMessage = "Email là bắt buộc")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        public string Email { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
//        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
//        public string Password { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
//        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
//        public string ReEnterPassword { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Họ tên là bắt buộc")]
//        public string FullName { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
//        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
//        public string PhoneNumber { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Giới tính là bắt buộc")]
//        public string Gender { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
//        public DateTime? DateOfBirth { get; set; }

//        public string? Address { get; set; }

//        public string? EducationLevel { get; set; }

//        public string? Company { get; set; }

//        public string? Occupation { get; set; }
//    }
//}
// File: GestPipe. Backend/Models/DTOs/Auth/RegisterDto.cs
using GestPipe.Backend.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Models.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ReEnterPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^[\d\s\+\-\(\)]+$", ErrorMessage = "Phone number contains invalid characters")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [ValidDateOfBirth(13, 120)] // ✅ Thêm validation này
        public DateTime? DateOfBirth { get; set; }

        // ✅ Optional fields - không cần [Required]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Education level cannot exceed 50 characters")]
        public string? EducationLevel { get; set; }

        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string? Company { get; set; }

        [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
        public string? Occupation { get; set; }
    }
}