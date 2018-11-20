using System;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IImageHelper
    {
        [CanBeNull]
        ImageSource Convert([NotNull] Uri target, [NotNull] string assembly);

        [CanBeNull]
        ImageSource Convert([NotNull] string uri, [NotNull] string assembly);
    }
}