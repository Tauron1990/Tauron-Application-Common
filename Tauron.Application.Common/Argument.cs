using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NLog;

namespace Tauron
{
    [PublicAPI]
    [DebuggerStepThrough]
    public static class Argument
    {
        public static bool LogErrors { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentClassName() => new StackFrame(3).GetMethod().DeclaringType?.FullName;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(Func<Exception> toCheck, Func<string> name)
        {
            var ex = toCheck();

            if(ex == null) return;

            if(LogErrors && LogManager.IsLoggingEnabled()) 
                LogManager.GetLogger(name()).Log(LogLevel.Error, ex);
            throw ex;
        }

        [MethodImpl(MethodImplOptions.NoInlining), NotNull]
        public static TType NotNull<TType>(TType toCheck, string parameterName)
        {
            Check(() => toCheck == null ? new ArgumentNullException(parameterName) : null, GetCurrentClassName);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TType NotNull<TType>(TType toCheck, string parameterName, string message)
        {
            Check(() => toCheck == null ? new ArgumentNullException(parameterName, message) : null, GetCurrentClassName);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string NotNull(string toCheck, string parameterName)
        {
            Check(() => string.IsNullOrWhiteSpace(toCheck) ? new ArgumentNullException(parameterName) : null, GetCurrentClassName);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(Func<Exception> toCheck) => Check(toCheck, GetCurrentClassName);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(bool toCheck, Func<Exception> exceptionBuilder) => Check(() => toCheck ? exceptionBuilder() : null, GetCurrentClassName);

        [MethodImpl(MethodImplOptions.NoInlining), NotNull]
        public static TValue CheckResult<TValue>(TValue value, string name)
        {
            Check(() => value == null ? new ArgumentNullException(name) : null, GetCurrentClassName);
            return value;
        }
    }
}