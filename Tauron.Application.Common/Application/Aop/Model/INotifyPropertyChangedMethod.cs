#region

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The NotifyPropertyChangedMethod interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (INotifyPropertyChangedMethodContracts))]
    public interface INotifyPropertyChangedMethod : INotifyPropertyChanged
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        void OnPropertyChanged([NotNull] string eventArgs);

        #endregion
    }

    [ContractClassFor(typeof (INotifyPropertyChangedMethod))]
    internal abstract class INotifyPropertyChangedMethodContracts : INotifyPropertyChangedMethod
    {
        #region Public Events

        /// <summary>The property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public void OnPropertyChanged(string eventArgs)
        {
            Contract.Requires<ArgumentNullException>(eventArgs != null, "eventArgs");
        }

        #endregion
    }
}