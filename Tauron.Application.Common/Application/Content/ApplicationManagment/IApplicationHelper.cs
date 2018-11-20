using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IApplicationHelper
    {
        [NotNull]
        [PublicAPI]
        Thread CreateUIThread([NotNull] ThreadStart start);

        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [PublicAPI]
        IWindow RunAnonymousApplication<T>() where T : class, IWindow;

        [PublicAPI]
        void RunAnonymousApplication([NotNull] IWindow window);
        
        [PublicAPI]
        void RunUIThread([NotNull] ThreadStart start);
    }
}