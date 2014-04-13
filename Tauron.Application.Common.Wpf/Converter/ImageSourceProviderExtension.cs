#region

using System;
using System.Windows.Markup;
using System.Windows.Media;
using Tauron.Application.Composition;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    /// <summary>The image source provider extension.</summary>
    [MarkupExtensionReturnType(typeof (ImageSource))]
    [PublicAPI]
    public sealed class ImageSourceProviderExtension : MarkupExtension
    {
        #region Public Properties

        [NotNull]
        public string Assembly { get; set; }

        [NotNull]
        public string ImageSource { get; set; }

        #endregion

        public ImageSourceProviderExtension()
        {
            Assembly = "this";
        }

        #region Public Methods and Operators

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ImageSourceHelper.Enter(ImageSource, serviceProvider)) return null;

            Assembly = ImageSourceHelper.ResolveAssembly(Assembly, serviceProvider);

            ImageSource temp = CompositionServices.Container.Resolve<IImageHelper>()
                                                  .Convert(ImageSource, Assembly);

            return ImageSourceHelper.Exit(ImageSource, temp == null) ? null : temp;
        }

        #endregion
    }
}