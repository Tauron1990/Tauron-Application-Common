// The file ImageHelper.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="ImageHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The image helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implementation
{
    /// <summary>The image helper.</summary>
    [Export(typeof (IImageHelper))]
    public class ImageHelper : IImageHelper
    {
        #region Fields

        private readonly WeakReferenceCollection<KeyedImage> _cache = new WeakReferenceCollection<KeyedImage>();

        [Inject] private IPackUriHelper _packUriHelper;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        public ImageSource Convert(Uri target, string assembly)
        {
            KeyedImage source = _cache.FirstOrDefault(img => img.Key == target);
            if (source != null)
            {
                ImageSource temp = source.GetImage();
                if (temp != null) return temp;
            }

            bool flag = target.IsAbsoluteUri && target.Scheme == Uri.UriSchemeFile && target.OriginalString.ExisFile();
            if (!flag) flag = target.IsAbsoluteUri;

            if (!flag) flag = target.OriginalString.ExisFile();

            if (flag)
            {
                ImageSource imgSource = BitmapFrame.Create(target);
                _cache.Add(new KeyedImage(target, imgSource));
                return imgSource;
            }

            try
            {
                return BitmapFrame.Create(_packUriHelper.LoadStream(target.OriginalString, assembly));
            }
            catch(Exception e)
            {
                CommonWpfConstans.LogCommon(true, "ImageHelper: Faild To Create image: {0}", e);

                return null;
            }
        }

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [CanBeNull]
        public ImageSource Convert([NotNull] string uri, [NotNull] string assembly)
        {
            Uri target;
            return Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out target) ? Convert(target, assembly) : null;
        }

        #endregion

        private class KeyedImage : IWeakReference
        {
            #region Fields

            private readonly WeakReference _source;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="KeyedImage" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="KeyedImage" /> Klasse.
            /// </summary>
            /// <param name="key">
            ///     The key.
            /// </param>
            /// <param name="source">
            ///     The source.
            /// </param>
            public KeyedImage(Uri key, ImageSource source)
            {
                Key = key;
                _source = new WeakReference(source);
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the key.</summary>
            public Uri Key { get; private set; }

            /// <summary>Gets a value indicating whether is alive.</summary>
            public bool IsAlive
            {
                get { return _source.IsAlive; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The get image.</summary>
            /// <returns>
            ///     The <see cref="ImageSource" />.
            /// </returns>
            public ImageSource GetImage()
            {
                return _source.Target as ImageSource;
            }

            #endregion
        }
    }
}