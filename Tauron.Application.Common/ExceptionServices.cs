// The file ExceptionServices.cs is part of Tauron.Application.Common.
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

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionServices.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The critical exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        public static bool IsCriticalApplicationException(Exception ex)
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
        public static bool IsCriticalException(Exception ex)
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
        public static Exception Unwrap(Exception ex)
        {
            Contract.Requires<ArgumentNullException>(ex != null, "ex");

            while (ex.InnerException != null && ex is TargetInvocationException) ex = ex.InnerException;

            return ex;
        }

        #endregion
    }
}