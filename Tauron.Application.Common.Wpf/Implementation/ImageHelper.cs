#region

using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implementation
{
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

        public ImageSource Convert(string uri, string assembly)
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

            public KeyedImage([NotNull] Uri key, [NotNull] ImageSource source)
            {
                Key = key;
                _source = new WeakReference(source);
            }

            #endregion

            #region Public Properties

            [NotNull]
            public Uri Key { get; private set; }

            public bool IsAlive
            {
                get { return _source.IsAlive; }
            }

            #endregion

            #region Public Methods and Operators

            [CanBeNull]
            public ImageSource GetImage()
            {
                return _source.Target as ImageSource;
            }

            #endregion
        }
    }
}