using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    public abstract class CommandLineCommandBase : ICommandLineCommand
    {
        protected CommandLineCommandBase([NotNull] string comandName) => CommandName = Argument.NotNull(comandName, nameof(comandName));

        public virtual void Execute(string[] args, IContainer container) => CommonConstants.LogCommon(false, "Command: {0} Empty Executet", CommandName);

        public string CommandName { get; private set; }

        public IShellFactory Factory { get; protected set; }

    }
}