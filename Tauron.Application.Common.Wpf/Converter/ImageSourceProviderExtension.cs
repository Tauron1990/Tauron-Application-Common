// The file ImageSourceProviderExtension.cs is part of Tauron.Application.Common.Wpf.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSourceProviderExtension.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The image source provider extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

        /// <summary>Gets or sets the assembly.</summary>
        public string Assembly { get; set; }

        /// <summary>Gets or sets the image source.</summary>
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

            ImageSource temp = CompositionServices.Container.Resolve<IImageHelper>()
                                                  .Convert(ImageSource, Assembly);

            return ImageSourceHelper.Exit(ImageSource, temp == null) ? null : temp;
        }

        #endregion
    }
}