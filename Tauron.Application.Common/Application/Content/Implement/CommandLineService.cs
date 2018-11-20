using System.Collections.Generic;
using Tauron.Application.Ioc;

namespace Tauron.Application.Implement
{
    [Export(typeof(ICommandLineService))]
    public class CommandLineService : ICommandLineService
    {
        [Inject]
        private readonly List<ICommandLineCommand> _commands = new List<ICommandLineCommand>();

        public IEnumerable<ICommandLineCommand> Commands => _commands.AsReadOnly();

        public void Add(ICommandLineCommand command) => _commands.Add(command);
    }
}