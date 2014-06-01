#region

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The critical exceptions.</summary>
    [PublicAPI]
    public static class CriticalExceptions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The is critical application exception.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsCriticalApplicationException([NotNull] Exception ex)
        {
            Contract.Requires<ArgumentNullException>(ex != null, "ex");

            ex = Unwrap(ex);
            return ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException
                   || ex is SecurityException;
        }

        /// <summary>
        ///     The is critical exception.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsCriticalException([NotNull] Exception ex)
        {
            Contract.Requires<ArgumentNullException>(ex != null, "ex");

            ex = Unwrap(ex);
            return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException
                   || ex is ThreadAbortException || ex is SEHException || ex is SecurityException;
        }

        /// <summary>
        ///     The unwrap.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <returns>
        ///     The <see cref="Exception" />.
        /// </returns>
        [NotNull]
        public static Exception Unwrap([NotNull] Exception ex)
        {
            Contract.Requires<ArgumentNullException>(ex != null, "ex");

            while (ex.InnerException != null && ex is TargetInvocationException) ex = ex.InnerException;

            return ex;
        }

        #endregion
    }
}