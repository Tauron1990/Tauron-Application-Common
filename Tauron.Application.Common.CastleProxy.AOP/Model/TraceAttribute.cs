using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Model
{
    [Flags]
    [PublicAPI]
    public enum TraceAspectOptions
    {
        None = 0,
        ParameterType = 1,
        ParameterName = 2,
        ParameterValue = 4,
        ReturnValue = 8
    }

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class TraceAttribute : AspectBaseAttribute
    {
        public TraceAttribute()
        {
            Order = 100;
            TraceEventType = LogLevel.Info;
            LogOptions = TraceAspectOptions.ParameterName;
            LogTitle = string.Empty;
        }

        private class LoggerHelper
        {
            public LoggerHelper(bool value, bool type)
            {
                _value = value;
                _type = type;
            }

            [CanBeNull]
            private string[] ParmNames { get; set; }

            public void Log([NotNull] LogEventInfo entry, [NotNull] IInvocation invocation)
            {
                lock (this)
                {
                    if (!_initialized)
                        Initialize(invocation.Method);
                }

                entry.Properties["Parameters"] = _stringNmes;
                entry.Properties["ParameterTypes"] = _types;

                if (!_value) return;

                var args = invocation.Arguments;
                for (var i = 0; i < ParmNames?.Length; i++) entry.Properties["Parameter:" + ParmNames[i]] = args[i];
            }

            private void Initialize([NotNull] MethodInfo info)
            {
                var parms = info.GetParameters();
                ParmNames = parms.Select(parm => parm.Name).ToArray();
                _stringNmes = ParmNames.Aggregate((working, next) => working + ", " + next);
                if (_type)
                {
                    _types = parms.Select(parm => parm.ParameterType)
                        .Aggregate("Types: ", (s, type1) => s + type1.ToString() + ", ");
                }

                _initialized = true;
            }

            private readonly bool _type;
            private readonly bool _value;
            private bool _initialized;
            private string _stringNmes;
            private string _types;
        }

        private LoggerHelper _helper;

        private bool _logReturn;

        public TraceAspectOptions LogOptions { get; set; }

        [CanBeNull]
        public string LogTitle { get; set; }

        public LogLevel TraceEventType { get; set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            base.Initialize(target, context, contextName);

            var logParameterName = LogOptions.HasFlag(TraceAspectOptions.ParameterName);
            var logParameterType = LogOptions.HasFlag(TraceAspectOptions.ParameterType);
            var logparameterValue = LogOptions.HasFlag(TraceAspectOptions.ParameterValue);

            _logReturn = LogOptions.HasFlag(TraceAspectOptions.ReturnValue);

            if (logParameterName || logParameterType || logparameterValue) _helper = new LoggerHelper(logparameterValue, logParameterType);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var logger = LogManager.GetLogger(LogTitle, invocation.TargetType);

            var isLoggingEnabled = LogManager.IsLoggingEnabled();
            if (isLoggingEnabled)
            {
                var entry = LogEventInfo.Create(TraceEventType, LogTitle, $"Enter Method: {invocation.Method.Name}");

                _helper?.Log(entry, invocation);

                logger.Log(entry);
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }

            if (!isLoggingEnabled) return;

            var entry2 = LogEventInfo.Create(TraceEventType, LogTitle, $"Exit Method: {invocation.Method.Name}");

            entry2.Properties["ReturnValue"] = invocation.ReturnValue;

            logger.Log(entry2);
        }
    }
}