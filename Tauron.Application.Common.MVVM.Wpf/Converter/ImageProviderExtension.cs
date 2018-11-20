using System;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;
using Tauron.Application.Composition;

namespace Tauron.Application.Converter
{
    [MarkupExtensionReturnType(typeof(Image))]
    [PublicAPI]
    public sealed class ImageProviderExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Assembly) || string.IsNullOrEmpty(ImageSource)) return null;

            if (ImageSourceHelper.Enter(ImageSource, serviceProvider)) return null;

            Assembly = ImageSourceHelper.ResolveAssembly(Assembly, serviceProvider);

            var temp = CompositionServices.Container.Resolve<IImageHelper>()
                    .Convert(new Uri(ImageSource, UriKind.RelativeOrAbsolute), Assembly);

            return ImageSourceHelper.Exit(ImageSource, temp == null) ? null : new Image {Source = temp};
        }

        [CanBeNull]
        public string Assembly { get; set; }

        [CanBeNull]
        public string ImageSource { get; set; }
    }
}