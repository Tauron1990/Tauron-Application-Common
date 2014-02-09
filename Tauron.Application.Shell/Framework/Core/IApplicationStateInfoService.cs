using System;
using System.Collections.Generic;
using System.Text;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Core
{
    /// <summary>
    /// This service is used to summarize important information
    /// about the state of the application when an exception occurs.
    /// </summary>
    [PublicAPI]
    public interface IApplicationStateInfoService : IService
    {
        /// <summary>
        /// Registers a new method to be invoked to get information about the current state of the application.
        /// </summary>
        /// <param name="title">The title of the new state entry.</param>
        /// <param name="stateGetter">The method to be invoked to get the state value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
        /// <exception cref="ArgumentException">A state getter with the specified <paramref name="title"/> is already registered.</exception>
        void RegisterStateGetter([NotNull] string title, [CanBeNull] Func<object> stateGetter);

        /// <summary>
        /// Determines whether a state getter with the specified title is already registered.
        /// </summary>
        /// <param name="title">The title to look for.</param>
        /// <returns><c>true</c>, if a state getter with the specified title is already registered, otherwise <c>false</c>.</returns>
        bool IsRegistered([NotNull] string title);

        /// <summary>
        /// Unregisters a state getter.
        /// </summary>
        /// <param name="title">The title of the state entry to remove.</param>
        /// <returns><c>true</c> if the specified title was found and removed, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
        bool UnregisterStateGetter([NotNull] string title);

        /// <summary>
        /// Gets a snapshot of the current application state information from all registered state getters.
        /// </summary>
        /// <returns>A dictionary with the titles and results of all registered state getters.</returns>
        [NotNull,System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        IReadOnlyDictionary<string, object> GetCurrentApplicationStateInfo();

        /// <summary>
        /// Appends the current application state information from all registered state getters
        /// to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append the state information to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void AppendFormatted([NotNull] StringBuilder sb);
    }
}