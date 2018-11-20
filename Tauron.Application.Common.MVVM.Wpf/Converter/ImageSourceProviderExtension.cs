using System;
using System.Windows.Markup;
using System.Windows.Media;
using JetBrains.Annotations;
using Tauron.Application.Composition;

namespace Tauron.Application.Converter
{
    [MarkupExtensionReturnType(typeof(ImageSource))]
    [PublicAPI]
    public sealed class ImageSourceProviderExtension : MarkupExtension
    {
        public ImageSourceProviderExtension() => Assembly = "this";

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Assembly) || string.IsNullOrEmpty(ImageSource)) return null;

            if (ImageSourceHelper.Enter(ImageSource, serviceProvider)) return null;

            Assembly = ImageSourceHelper.ResolveAssembly(Assembly, serviceProvider);

            var temp = CompositionServices.Container.Resolve<IImageHelper>()
                .Convert(ImageSource, Assembly);

            return ImageSourceHelper.Exit(ImageSource, temp == null) ? null : temp;
        }

        [CanBeNull]
        public string Assembly { get; set; }

        [CanBeNull]
        public string ImageSource { get; set; }
    }
}