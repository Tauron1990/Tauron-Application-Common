using System;
using JetBrains.Annotations;


namespace Tauron.Application
{
    [PublicAPI]
    public class DefaultShellFactory : IShellFactory
    {
        private readonly Type _shellType;

        public DefaultShellFactory([NotNull] Type shellType) => _shellType = Argument.NotNull(shellType, nameof(shellType));

        public object CreateView() => Activator.CreateInstance(_shellType);
    }
}