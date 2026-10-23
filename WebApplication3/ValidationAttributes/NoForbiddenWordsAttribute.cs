using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApplication3.ValidationAttributes
{
    public class NoForbiddenWordsAttribute : ValidationAttribute
    {
        private readonly string[] _forbidden = { "хлам", "брак", "дешёвка", "убийца" };

        public NoForbiddenWordsAttribute()
        {
            ErrorMessage = "Название содержит недопустимое слово";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string name && _forbidden.Any(f => name.Contains(f)))
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}