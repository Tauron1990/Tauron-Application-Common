using System.Linq;

namespace Tauron.Application.Models.Rules
{
    public class CreditCardRule : ModelRule
    {
        public CreditCardRule()
            :base(nameof(CreditCardRule))
        {
            
        }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (value == null)
                return CreateResult();
            string text = value as string;
            if (text == null)
                return CreateResult(RuleMessages.CreditCardInvalid);
            text = text.Replace("-", "");
            text = text.Replace(" ", "");
            int num = 0;
            bool flag = false;
            foreach (char item in text.Reverse())
            {
                if (item < '0' || item > '9')
                    return CreateResult(RuleMessages.CreditCardInvalid);
                int num2 = (item - 48) * ((!flag) ? 1 : 2);
                flag = !flag;
                while (num2 > 0)
                {
                    num += num2 % 10;
                    num2 /= 10;
                }
            }

            return num % 10 == 0 ? CreateResult() : CreateResult(RuleMessages.CreditCardInvalid);
        }
    }
}