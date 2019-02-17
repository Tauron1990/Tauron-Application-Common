using Tauron.Application.Models.Rules;

namespace Tauron.Application.Models
{
    public static class ValidatorConfiguration
    {
        private static IValidator _validator;

        public static IValidator Validator
        {
            get => _validator ?? DefaultValidator.DefaultValidatorInstance;
            set => _validator = value;
        }
    }
}