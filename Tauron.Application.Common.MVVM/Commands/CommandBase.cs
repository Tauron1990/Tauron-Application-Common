using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Tauron.Application.Commands
{
    public abstract class CommandBase : ICommand
    {
        public virtual event EventHandler CanExecuteChanged
        {
            add => CommandManagerDelegator.Add(value);
            remove => CommandManagerDelegator.Remove(value);
        }

        public virtual bool CanExecute([CanBeNull] object parameter) => true;

        public abstract void Execute([CanBeNull] object parameter);

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [UsedImplicitly]
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void RaiseCanExecuteChanged()
        {
            // ReSharper restore VirtualMemberNeverOverriden.Global
            CommandManagerDelegator.InvalidateRequerySuggested();
        }
    }
}