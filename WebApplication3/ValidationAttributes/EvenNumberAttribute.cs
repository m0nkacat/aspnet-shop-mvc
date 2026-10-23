using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ValidationAttributes
{
    public class EvenNumberAttribute : ValidationAttribute
    {
        public EvenNumberAttribute()
        {
            ErrorMessage = "Значение должно быть чётным";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int number && number % 2 != 0)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}