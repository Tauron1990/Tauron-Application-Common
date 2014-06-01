#region

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