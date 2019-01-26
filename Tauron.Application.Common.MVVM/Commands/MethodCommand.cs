using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Commands
{
    public sealed class EventData
    {
        public EventData([NotNull] object sender, [NotNull] object eventArgs)
        {
            Sender = Argument.NotNull(sender, nameof(sender));
            EventArgs = Argument.NotNull(eventArgs, nameof(eventArgs));
        }

        [NotNull]
        public object EventArgs { get; }

        [NotNull]
        public object Sender { get; }

    }

    /// <summary>The method command.</summary>
    public sealed class MethodCommand : CommandBase
    {
        public MethodCommand([NotNull] MethodInfo method, [NotNull] WeakReference context)
        {
            _method = Argument.NotNull(method, nameof(method));
            _context = Argument.NotNull(context, nameof(context));

            _methodType = (MethodType) method.GetParameters().Length;
            if (_methodType != MethodType.One) return;
            if (_method.GetParameters()[0].ParameterType != typeof(EventData)) _methodType = MethodType.EventArgs;
        }

        [CanBeNull]
        private object Context => _context?.Target;

        public override void Execute(object parameter)
        {
            object[] args;

            var temp = (EventData) parameter;
            switch (_methodType)
            {
                case MethodType.Zero:
                    args = new object[0];
                    break;
                case MethodType.One:
                    args = new object[] {temp};
                    break;
                case MethodType.Two:
                    args = new[] {temp?.Sender, temp?.EventArgs};
                    break;
                case MethodType.EventArgs:
                    args = new[] {temp?.EventArgs};
                    break;
                default:
                    args = new object[0];
                    break;
            }

            _method.Invoke(Context, args);
        }

        private enum MethodType
        {
            Zero = 0,
            One,
            Two,
            EventArgs
        }

        private readonly WeakReference _context;
        private readonly MethodInfo _method;
        private readonly MethodType _methodType;
    }
}