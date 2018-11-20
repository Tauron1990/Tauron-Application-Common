using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface IPackUriHelper
    {
        [NotNull]
        string GetString([NotNull] string pack);

        [NotNull]
        string GetString([NotNull] string pack, [NotNull] string assembly, bool full);

        [NotNull]
        Uri GetUri([NotNull] string pack);

        [NotNull]
        Uri GetUri([NotNull] string pack, [NotNull] string assembly, bool full);

        [NotNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack) where T : class;

        [NotNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack, [NotNull] string assembly) where T : class;

        [NotNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        Stream LoadStream([NotNull] string pack);

        [NotNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        Stream LoadStream([NotNull] string pack, [NotNull] string assembly);

    }
}