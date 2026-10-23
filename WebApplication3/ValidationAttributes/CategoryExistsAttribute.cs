using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebApplication3.Models;

namespace WebApplication3.ValidationAttributes
{
    public class CategoryExistsAttribute : ValidationAttribute
    {
        public CategoryExistsAttribute()
        {
            ErrorMessage = "Категория с указанным ID не существует";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int categoryId && categoryId > 0)
            {
                var dbContext = validationContext.GetRequiredService<ShopDbContext>();
                var exists = dbContext.Categories.Any(c => c.CategoryId == categoryId);
                if (!exists)
                    return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}