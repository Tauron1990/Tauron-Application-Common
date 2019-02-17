using System.Collections.Generic;

namespace Tauron.Application.Models
{
    public interface IValidator
    {
        PropertyIssue[] Validate(ValidatorContext context, object value);
    }
}