using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Common.Attributes
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        public MinimumAgeAttribute(int minAge) => _minAge = minAge;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                // Calculate age accurately by comparing years and checking if birthday has passed
                DateTime today = DateTime.Today;
                int age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;

                if (age >= _minAge)
                    return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"You must be at least {_minAge} years old.");
        }
    }

}
