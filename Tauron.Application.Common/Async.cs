// The file Async.cs is part of Tauron.Application.Common.
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
// <copyright file="Async.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The async.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Ermöglich auf einfache weise eine Asyncrone Aufgabe Auszuführen.</summary>
    /// <remarks>
    ///     <para>Ermöglich auf einfache weise eine Asyncrone Aufgabe Auszuführen.</para>
    ///     <para>Intern wird auf die TPL zurüggeriffen.</para>
    /// </remarks>
    [PublicAPI]
    public static class Async
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Startet einen neuen Task mit dem Standart Verhalten.
        /// </summary>
        /// <example>
        ///     Beschpielhafte darstellen des Aufruhfes <see cref="StartNew" />
        ///     <code>
        /// Task task = new Action(DoSonmthing).StartNew();
        /// </code>
        /// </example>
        /// <param name="method">
        ///     Ein Action Delegate das dem neuen
        ///     <seealso cref="Task" />
        ///     übergeben wird.
        /// </param>
        /// <returns>
        ///     Der erstellte <see cref="Task" />.
        /// </returns>
        public static Task StartNew(this Action method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            return Task.Factory.StartNew(method);
        }

        /// <summary>
        ///     Startet einen neuen Task mit der Long Running Einstellung.
        /// </summary>
        /// <example>
        ///     Beschpielhafte darstellen des Aufruhfes <see cref="StartNewLong" />
        ///     <code>
        /// Task task = new Action(DoSonmthing).StartNewLong();
        /// </code>
        /// </example>
        /// <param name="method">
        ///     Ein Action Delegate das dem neuen
        ///     <seealso cref="Task" />
        ///     übergeben wird.
        /// </param>
        /// <returns>
        ///     Der erstellte <see cref="Task" />.
        /// </returns>
        public static Task StartNewLong(this Action method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            return Task.Factory.StartNew(method, TaskCreationOptions.LongRunning);
        }

        #endregion
    }
}