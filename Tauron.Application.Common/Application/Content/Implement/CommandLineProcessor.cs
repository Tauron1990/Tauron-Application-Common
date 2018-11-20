using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Implement
{
    [PublicAPI]
    public class CommandLineProcessor
    {
        public CommandLineProcessor([NotNull] CommonApplication application)
        {
            _application = Argument.NotNull(application, nameof(application));
            ParseCommandLine();
        }
        
        public class Command : IEquatable<Command>
        {
            public Command([NotNull] string name)
            {
                Name = Argument.NotNull(name, nameof(name));
                Parms = new List<string>();
            }
            
            public bool Equals(Command other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((Command) obj);
            }

            public override int GetHashCode() => Name.GetHashCode();

            public static bool operator ==(Command left, Command right) => Equals(left, right);

            public static bool operator !=(Command left, Command right) => !Equals(left, right);

            [NotNull]
            public string Name { get; }
            
            [NotNull]
            public List<string> Parms { get; }
            
        }
        
        private readonly CommonApplication _application;
        
        private HashSet<Command> _commands;
        
        private IShellFactory _factory;
        
        [CanBeNull]
        public object CreateShellView()
        {
            SelectViewFacotry();
            return _factory?.CreateView();
        }
        
        public void ExecuteCommands()
        {
            foreach (var fileCommand in
                _commands.Where(com => com.Name == "FileCommand").ToArray())
            {
                var commandPrcessor = _application.Container.Resolve<IFileCommand>(null, true);
                if (commandPrcessor == null) break;

                var file = fileCommand.Parms[0];

                commandPrcessor.ProcessFile(file);
                _factory = commandPrcessor.ProvideFactory();
            }

            foreach (var command in
                _application.Container.Resolve<ICommandLineService>().Commands)
            {
                var command1 = command;
                var temp = _commands.FirstOrDefault(arg => arg.Name == command1.CommandName);
                if (temp == null) continue;

                command.Execute(temp.Parms.ToArray(), _application.Container);
            }

            _commands = null;
        }
        
        private void ParseCommandLine() => _commands = new HashSet<Command>(ParseCommandLine(_application.GetArgs()).Where(c => c != null));

        public static IEnumerable<Command> ParseCommandLine(IEnumerable<string> args, bool skipfirst = true)
        {
            Command current = null;
            var first = skipfirst;
            foreach (var arg in args)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                if (current == null && arg.ExisFile())
                {
                    var temp = new Command("FileCommand");
                    temp.Parms.Add(arg);
                    yield return temp;
                }

                if (arg.StartsWith("-", StringComparison.Ordinal))
                {
                    if (current != null) yield return current;

                    current = new Command(arg.TrimStart('-'));
                }
                else
                {
                    current?.Parms.Add(arg);
                }
            }

            yield return current;
        }
        
        private void SelectViewFacotry()
        {
            if (_factory != null) return;

            foreach (var command in
                _application.Container.Resolve<ICommandLineService>()
                    .Commands.Where(
                        command =>
                            command.Factory != null &&
                            _commands.Any(com => com.Name == command.CommandName))
            )
                _factory = command.Factory;
        }
    }
}