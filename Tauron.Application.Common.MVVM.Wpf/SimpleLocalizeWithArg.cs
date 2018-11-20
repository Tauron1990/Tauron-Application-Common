using System;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [MarkupExtensionReturnType(typeof(string))]
    public class SimpleLocalizeWithArg : SimpleLocalize
    {
        [CanBeNull]
        public string Arg { get; private set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var value = base.ProvideValue(serviceProvider) as string;

            if (string.IsNullOrWhiteSpace(value)) return "null";

            try
            {
                return value.SFormat(Arg);
            }
            catch (FormatException)
            {
                return "null";
            }
        }
    }
}