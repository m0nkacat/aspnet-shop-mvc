using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ValidationAttributes
{
    public class PricePositiveAttribute : ValidationAttribute
    {
        public PricePositiveAttribute()
        {
            ErrorMessage = "Цена должна быть больше 0";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is decimal price && price <= 0)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}