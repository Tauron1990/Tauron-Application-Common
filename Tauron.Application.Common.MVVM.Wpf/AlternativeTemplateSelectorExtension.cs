using System;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [MarkupExtensionReturnType(typeof(DataTemplateSelector))]
    [PublicAPI]
    public sealed class AlternativeTemplateSelectorExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => new AlternativTemplateSelector();
    }
}