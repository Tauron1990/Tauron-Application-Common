// The file TraceAttribute.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The trace aspect options.</summary>
    [Flags]
    [PublicAPI]
    public enum TraceAspectOptions
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The parameter type.</summary>
        ParameterType = 1,

        /// <summary>The parameter name.</summary>
        ParameterName = 2,

        /// <summary>The parameter value.</summary>
        ParameterValue = 4,

        /// <summary>The return value.</summary>
        ReturnValue = 8,
    }

    /// <summary>The trace attributte.</summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public sealed class TraceAttribute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _helper.</summary>
        private LoggerHelper _helper;

        /// <summary>The _log return.</summary>
        private bool _logReturn;

        /// <summary>The _tracer title.</summary>
        private string _tracerTitle;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TraceAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="TraceAttribute" /> class.
        /// </summary>
        public TraceAttribute()
        {
            Order = 100;
            TraceEventType = TraceEventType.Information;
            LogOptions = TraceAspectOptions.ParameterName;
            ExceptionPolicy = null;
            Category = string.Empty;
            LogTitle = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the category.</summary>
        /// <value>The category.</value>
        public string Category { get; set; }

        /// <summary>Gets or sets a value indicating whether enable tracer.</summary>
        /// <value>The enable tracer.</value>
        public bool EnableTracer { get; set; }

        /// <summary>Gets or sets the exception policy.</summary>
        /// <value>The exception policy.</value>
        public string ExceptionPolicy { get; set; }

        /// <summary>Gets or sets the log options.</summary>
        /// <value>The log options.</value>
        public TraceAspectOptions LogOptions { get; set; }

        /// <summary>Gets or sets the log title.</summary>
        /// <value>The log title.</value>
        public string LogTitle { get; set; }

        /// <summary>Gets or sets the trace event type.</summary>
        /// <value>The trace event type.</value>
        public TraceEventType TraceEventType { get; set; }

        /// <summary>Gets or sets the tracer id.</summary>
        /// <value>The tracer id.</value>
        public string TracerId { get; set; }

        /// <summary>Gets or sets the tracer title.</summary>
        /// <value>The tracer title.</value>
        public string TracerTitle
        {
            get
            {
                if (string.IsNullOrEmpty(_tracerTitle)) return LogTitle;

                return _tracerTitle;
            }

            set { _tracerTitle = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        protected internal override void Initialize(object target, ObjectContext context, string contextName)
        {
            base.Initialize(target, context, contextName);

            bool logParameterName = LogOptions.HasFlag(TraceAspectOptions.ParameterName);
            bool logParameterType = LogOptions.HasFlag(TraceAspectOptions.ParameterType);
            bool logparameterValue = LogOptions.HasFlag(TraceAspectOptions.ParameterValue);

            _logReturn = LogOptions.HasFlag(TraceAspectOptions.ReturnValue);

            if (logParameterName || logParameterType || logparameterValue) _helper = new LoggerHelper(logparameterValue, logParameterType);
        }

        /// <summary>
        ///     The intercept impl.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            bool isLoggingEnabled = Logger.IsLoggingEnabled();
            if (isLoggingEnabled)
            {
                var entry = new LogEntry
                {
                    Message =
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enter Method: {0}",
                            invocation.Method.Name),
                    Title = LogTitle
                };

                if (!string.IsNullOrEmpty(Category)) entry.Categories.Add(Category);

                if (_helper != null) _helper.Log(entry, invocation);

                Logger.Write(entry);
            }

            Tracer tracer = null;

            try
            {
                if (EnableTracer && isLoggingEnabled && Logger.Writer.IsTracingEnabled())
                {
                    if (TracerId != null)
                    {
                        Guid id;
                        if (Guid.TryParse(TracerId, out id)) tracer = new Tracer(TracerTitle, id);
                        else
                        {
                            Logger.Write(
                                string.Format("Unkown Activity ID: {0}", TracerId),
                                string.Empty,
                                -1,
                                1,
                                TraceEventType.Warning);
                            tracer = new Tracer(TracerTitle);
                        }
                    }
                    else tracer = new Tracer(TracerTitle);
                }

                invocation.Proceed();
            }
            catch (Exception e)
            {
                if (ExceptionPolicy == null) throw;

                Exception newEx;
                if (Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(
                    e,
                    ExceptionPolicy,
                    out newEx))
                {
                    if (newEx != null) throw newEx;

                    throw;
                }
            }
            finally
            {
                if (tracer != null) tracer.Dispose();
            }

            if (!isLoggingEnabled) return;

            var logEntry2 = new LogEntry {Message = string.Format("Exit Method: {0}", invocation.Method.Name)};
            if (_logReturn) logEntry2.ExtendedProperties["ReturnValue"] = invocation.ReturnValue;

            Logger.Write(logEntry2);
        }

        #endregion

        /// <summary>The logger helper.</summary>
        private class LoggerHelper
        {
            #region Fields

            /// <summary>The _type.</summary>
            private readonly bool _type;

            /// <summary>The _value.</summary>
            private readonly bool _value;

            /// <summary>The _initialized.</summary>
            private bool _initialized;

            /// <summary>The _string nmes.</summary>
            private string _stringNmes;

            /// <summary>The _types.</summary>
            private string _types;

            /// <summary>The _parm names.</summary>
            private string[] parmNames;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LoggerHelper" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LoggerHelper" /> Klasse.
            ///     Initializes a new instance of the <see cref="LoggerHelper" /> class.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <param name="type">
            ///     The type.
            /// </param>
            public LoggerHelper(bool value, bool type)
            {
                _value = value;
                _type = type;
            }

            #endregion

            #region Properties

            /// <summary>The _parm names.</summary>
            private string[] ParmNames
            {
                get
                {
                    Contract.Requires(parmNames != null);
                    Contract.Ensures(Contract.Result<string[]>() != null);

                    return parmNames;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The log.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <param name="invocation">
            ///     The invocation.
            /// </param>
            [ContractVerification(false)]
            public void Log(LogEntry entry, IInvocation invocation)
            {
                Contract.Requires<ArgumentNullException>(entry != null, "entry");
                Contract.Requires<ArgumentNullException>(invocation != null, "invocation");

                lock (this)
                {
                    if (!_initialized) Initialize(invocation.Method);
                }

                entry.ExtendedProperties["Parameters"] = _stringNmes;
                entry.ExtendedProperties["ParameterTypes"] = _types;

                if (!_value) return;

                object[] args = invocation.Arguments;
                for (int i = 0; i < ParmNames.Length; i++) entry.ExtendedProperties["Parameter:" + ParmNames[i]] = args[i];
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The initialize.
            /// </summary>
            /// <param name="info">
            ///     The info.
            /// </param>
            private void Initialize(MethodInfo info)
            {
                Contract.Requires<ArgumentNullException>(info != null, "info");

                ParameterInfo[] parms = info.GetParameters();
                parmNames = parms.Select(parm => parm.Name).ToArray();
                _stringNmes = ParmNames.Aggregate((working, next) => working + ", " + next);
                if (_type)
                {
                    _types = parms.Select(parm => parm.ParameterType)
                                  .Aggregate("Types: ", (s, type1) => s + type1.ToString() + ", ");
                }

                _initialized = true;
            }

            #endregion
        }
    }
}