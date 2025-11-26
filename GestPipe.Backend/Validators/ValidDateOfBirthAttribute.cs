// File: GestPipe.Backend/Validators/ValidDateOfBirthAttribute.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace GestPipe.Backend.Validators
{
    public class ValidDateOfBirthAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;
        private readonly int _maximumAge;

        public ValidDateOfBirthAttribute(int minimumAge = 13, int maximumAge = 120)
        {
            _minimumAge = minimumAge;
            _maximumAge = maximumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date of birth is required");
            }

            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;

                if (dateOfBirth.Date > today.AddYears(-age))
                    age--;

                if (dateOfBirth > today)
                {
                    return new ValidationResult("Date of birth cannot be in the future");
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old to register");
                }

                if (age > _maximumAge)
                {
                    return new ValidationResult($"Invalid date of birth");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid date format");
        }
    }
}