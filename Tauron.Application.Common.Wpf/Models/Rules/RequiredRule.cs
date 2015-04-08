using System.Windows.Media.Media3D;

namespace Tauron.Application.Models.Rules
{
    public sealed class RequiredRule : ModelRule
    {
        public bool AllowStringEmpty { get; set; }

        public RequiredRule()
        {
            Id = "RequiredRule";
            Message = () => ResourceMessages.RequireRuleError;
        }

        public override bool IsValidValue(object obj, ValidatorContext context)
        {
            if(obj == null) return false;
            var str = obj as string;
            
            if (str == null || AllowStringEmpty) return true;

            return !string.IsNullOrWhiteSpace(str);
        }
    }
}