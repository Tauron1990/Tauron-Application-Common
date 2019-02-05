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
        public static void Check<TType>(Func<TType, string, Exception> toCheck, Func<string> name, TType target, string parameterName)
        {
            var ex = toCheck(target, parameterName);

            if(ex == null) return;

            if(LogErrors && LogManager.IsLoggingEnabled()) 
                LogManager.GetLogger(name()).Log(LogLevel.Error, ex);
            throw ex;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check<TType>(Func<TType, string, string, Exception> toCheck, Func<string> name, TType target, string message, string parameterName)
        {
            var ex = toCheck(target, message, parameterName);

            if (ex == null) return;

            if (LogErrors && LogManager.IsLoggingEnabled())
                LogManager.GetLogger(name()).Log(LogLevel.Error, ex);
            throw ex;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(Func<Exception> toCheck, Func<string> name)
        {
            var ex = toCheck();

            if (ex == null) return;

            if (LogErrors && LogManager.IsLoggingEnabled())
                LogManager.GetLogger(name()).Log(LogLevel.Error, ex);
            throw ex;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check<TValue>(Func<TValue, Func<Exception>, Exception> toCheck, Func<string> name, TValue value, Func<Exception> builder)
        {
            var ex = toCheck(value, builder);

            if (ex == null) return;

            if (LogErrors && LogManager.IsLoggingEnabled())
                LogManager.GetLogger(name()).Log(LogLevel.Error, ex);
            throw ex;
        }


        private static Exception ToCheck<TType>(TType toCheck, string parameterName) => toCheck == null ? new ArgumentNullException(parameterName) : null;

        [MethodImpl(MethodImplOptions.NoInlining), NotNull]
        public static TType NotNull<TType>(TType toCheck, string parameterName)
        {
            Check(ToCheck, GetCurrentClassName, toCheck, parameterName);
            return toCheck;
        }

        private static Exception ToCheck<TType>(TType toCheck, string message, string parameterName) => toCheck == null ? new ArgumentNullException(parameterName, message) : null;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TType NotNull<TType>(TType toCheck, string parameterName, string message)
        {
            Check(ToCheck, GetCurrentClassName, toCheck, message, parameterName);
            return toCheck;
        }

        private static Exception ToCheck(string toCheck, string parameterName) => string.IsNullOrWhiteSpace(toCheck) ? new ArgumentNullException(parameterName) : null;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string NotNull(string toCheck, string parameterName)
        {
            Check(ToCheck, GetCurrentClassName, toCheck, parameterName);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(Func<Exception> toCheck) => Check(toCheck, GetCurrentClassName);

        public static Exception ToCheck(bool toCheck, Func<Exception> exceptionBuilder) => toCheck ? exceptionBuilder() : null;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(bool toCheck, Func<Exception> exceptionBuilder)
        {
            Check(ToCheck, GetCurrentClassName, toCheck, exceptionBuilder);
        }

        [MethodImpl(MethodImplOptions.NoInlining), NotNull]
        public static TValue CheckResult<TValue>(TValue value, string name)
        {
            Check(ToCheck, GetCurrentClassName, value, name);
            return value;
        }
    }
}