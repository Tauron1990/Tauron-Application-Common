namespace Tauron.Application.Models
{
    public class ValidatorResult
    {
        public string Message { get; }

        public bool Succseeded { get; }

        public ValidatorResult(string message, bool succseeded)
        {
            Message = message;
            Succseeded = succseeded;
        }
    }
}