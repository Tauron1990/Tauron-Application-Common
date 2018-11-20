using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Implementation
{
    [Export(typeof(IImageHelper))]
    public class ImageHelper : IImageHelper
    {
        private class KeyedImage : IWeakReference
        {
            private readonly WeakReference _source;

            public KeyedImage([NotNull] Uri key, [NotNull] ImageSource source)
            {
                Key = key;
                _source = new WeakReference(source);
            }

            [CanBeNull]
            public ImageSource GetImage() => _source.Target as ImageSource;

            [NotNull]
            public Uri Key { get; }

            public bool IsAlive => _source.IsAlive;

        }

        private readonly WeakReferenceCollection<KeyedImage> _cache = new WeakReferenceCollection<KeyedImage>();

        [Inject]
        private IPackUriHelper _packUriHelper;

        public ImageSource Convert(Uri target, string assembly)
        {
            var source = _cache.FirstOrDefault(img => img.Key == target);
            var temp = source?.GetImage();
            if (temp != null) return temp;

            var flag = target.IsAbsoluteUri && target.Scheme == Uri.UriSchemeFile && target.OriginalString.ExisFile();
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
            catch (Exception e)
            {
                CommonWpfConstans.LogCommon(true, "ImageHelper: Faild To Create image: {0}", e);

                return null;
            }
        }

        public ImageSource Convert(string uri, string assembly) => Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out var target) ? Convert(target, assembly) : null;
    }
}