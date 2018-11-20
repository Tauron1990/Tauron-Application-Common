using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IModule
    {
        int Order { get; }

        void Initialize([NotNull] CommonApplication application);
    }
}