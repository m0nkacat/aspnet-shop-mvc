using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ValidationAttributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
        {
            ErrorMessage = "Дата не должна быть в прошлом";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date < DateTime.Now.Date)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}