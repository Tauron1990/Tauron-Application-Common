#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.Application.Aop.Model;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The property helper.</summary>
    public static class PropertyHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The extract property name.
        /// </summary>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull,SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>([NotNull] Expression<Func<T>> propertyExpression)
        {
            Contract.Requires<ArgumentNullException>(propertyExpression != null, "propertyExpression");

            var memberExpression = (MemberExpression) propertyExpression.Body;
            
            return memberExpression.Member.Name;
        }

        #endregion
    }

    /// <summary>The observable object.</summary>
    [Serializable, DebuggerNonUserCode]
    [PublicAPI]
    public abstract class ObservableObject : EventListManager, INotifyPropertyChangedMethod
    {
        [PublicAPI]
        protected class LogHelper
        {
            private readonly ObservableObject _target;

            public LogHelper([NotNull] ObservableObject target)
            {
                _target = target;
            }

            public void Write([CanBeNull] object message, TraceEventType type)
            {
                if (Logger.IsLoggingEnabled()) Logger.Write(message, _target.Category, -1, -1, type);
            }

            [StringFormatMethod("format")]
            public void WriteFormat(TraceEventType type, [NotNull] string format, [NotNull] params object[] parms)
            {
                Write(string.Format(format, parms), type);
            }
        }

        #region Public Events

        /// <summary>The property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { AddEvent("PropertyChangedEventHandler", value); }

            remove { RemoveEvent("PropertyChangedEventHandler", value); }
        }

        #endregion

        protected ObservableObject()
        {
            Category = GetType().ToString();
        }

        #region Public Properties

        private LogHelper _logHelper;

        /// <summary>Gets the current dispatcher.</summary>
        /// <value>The current dispatcher.</value>
        [NotNull]
        public static IUISynchronize CurrentDispatcher => UiSynchronize.Synchronize;

        [NotNull]
        protected LogHelper Log => _logHelper ?? (_logHelper = new LogHelper(this));

        [CanBeNull]
        protected string Category { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public virtual void OnPropertyChanged([CallerMemberName] string eventArgs = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(eventArgs));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public virtual void OnPropertyChanged([NotNull] PropertyChangedEventArgs eventArgs)
        {
            Contract.Requires<ArgumentNullException>(eventArgs != null, "eventArgs");

            InvokeEvent("PropertyChangedEventHandler", this, eventArgs);
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void OnPropertyChanged<T>([NotNull] Expression<Func<T>> eventArgs)
        {
            Contract.Requires<ArgumentNullException>(eventArgs != null, "eventArgs");

            OnPropertyChanged(new PropertyChangedEventArgs(PropertyHelper.ExtractPropertyName(eventArgs)));
        }


        public virtual void OnPropertyChangedExplicit([NotNull] string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null, "propertyName");

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The queue workitem.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <param name="withDispatcher">
        ///     The with dispatcher.
        /// </param>
        protected static void QueueWorkitem([NotNull] Action action, bool withDispatcher)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");

            CommonApplication.Scheduler.QueueTask(new UserTask(action, withDispatcher));
        }

        #endregion
    }
}