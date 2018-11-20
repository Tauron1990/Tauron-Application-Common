using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    public static class PropertyHelper
    {
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>([NotNull] Expression<Func<T>> propertyExpression)
        {
            Argument.NotNull(propertyExpression, nameof(propertyExpression));
            var memberExpression = (MemberExpression) propertyExpression.Body;

            return memberExpression.Member.Name;
        }
    }

    [Serializable]
    [PublicAPI]
    public abstract class ObservableObject : BaseObject, INotifyPropertyChangedMethod
    {
        protected ObservableObject()
        {
            LogName = GetType().Name;
            LogCategory = GetType().ToString();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected static void QueueWorkitem([NotNull] Action action, bool withDispatcher = false) => CommonApplication.Scheduler.QueueTask(new UserTask(Argument.NotNull(action, nameof(action)), withDispatcher));

        [PublicAPI]
        protected class LogHelper
        {
            private readonly string _name;
            private Logger _logger;

            public LogHelper(string name) => _name = name;

            public Logger Logger => _logger ?? (_logger = LogManager.GetLogger(_name));

            public void Write([CanBeNull] object message, LogLevel type) => Logger.Log(type, message);

            [StringFormatMethod("format")]
            public void Write(LogLevel type, [NotNull] string format, [NotNull] params object[] parms) => Logger.Log(type, format, parms);

            public void Error(Exception e, string messege) => Logger.Error(e, messege);
        }
        
        private LogHelper _logHelper;
        
        [NotNull]
        public static IUISynchronize CurrentDispatcher => UiSynchronize.Synchronize;

        [NotNull]
        protected LogHelper Log => _logHelper ?? (_logHelper = new LogHelper(LogName));

        [CanBeNull]
        protected string LogCategory { get; set; }

        public string LogName { get; set; }
        
        public void SetProperty<TType>(ref TType property, TType value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name, nameof(name)));
        }

        public void SetProperty<TType>(ref TType property, TType value, Action changed, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name, nameof(name)));
            changed();
        }
        
        // ReSharper disable once AssignNullToNotNullAttribute
        public virtual void OnPropertyChanged([CallerMemberName] string eventArgs = null) => OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(eventArgs, nameof(eventArgs))));

        public virtual void OnPropertyChanged([NotNull] PropertyChangedEventArgs eventArgs) => OnPropertyChanged(this, Argument.NotNull(eventArgs, nameof(eventArgs)));

        public virtual void OnPropertyChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs eventArgs) => 
            PropertyChanged?.Invoke(Argument.NotNull(sender, nameof(sender)), Argument.NotNull(eventArgs, nameof(eventArgs)));

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void OnPropertyChanged<T>([NotNull] Expression<Func<T>> eventArgs) =>
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyHelper.ExtractPropertyName(Argument.NotNull(eventArgs, nameof(eventArgs)))));


        public virtual void OnPropertyChangedExplicit([NotNull] string propertyName) => 
            OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(propertyName, nameof(propertyName))));
    }
}