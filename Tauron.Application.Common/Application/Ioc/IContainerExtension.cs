using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public interface IContainerExtension
    {
        void Initialize([NotNull] ComponentRegistry components);
    }
}