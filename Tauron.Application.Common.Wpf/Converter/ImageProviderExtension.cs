﻿#region

using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Tauron.Application.Composition;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    /// <summary>The image provider extension.</summary>
    [MarkupExtensionReturnType(typeof (Image))]
    [PublicAPI]
    public sealed class ImageProviderExtension : MarkupExtension
    {
        #region Public Properties

        /// <summary>Gets or sets the assembly.</summary>
        [NotNull]
        public string Assembly { get; set; }

        /// <summary>Gets or sets the image source.</summary>
        [NotNull]
        public string ImageSource { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ImageSourceHelper.Enter(ImageSource, serviceProvider)) return null;

            Assembly = ImageSourceHelper.ResolveAssembly(Assembly, serviceProvider);

            ImageSource temp =
                CompositionServices.Container.Resolve<IImageHelper>()
                                   .Convert(new Uri(ImageSource, UriKind.RelativeOrAbsolute), Assembly);

            return ImageSourceHelper.Exit(ImageSource, temp == null) ? null : new Image {Source = temp};
        }

        #endregion
    }
}