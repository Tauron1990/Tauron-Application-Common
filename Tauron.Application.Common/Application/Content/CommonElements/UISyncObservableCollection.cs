#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The ui sync observable collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode, PublicAPI, Serializable]
    public class UISyncObservableCollection<TType> : ObservableCollection<TType>
    {
        private class DummySync : IUISynchronize
        {
            public Task BeginInvoke(Action action)
            {
                action();
// ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }

            public void Invoke(Action action)
            {
                action();
            }

            public TReturn Invoke<TReturn>(Func<TReturn> action)
            {
                return action();
            }
        }

        private IUISynchronize _synchronize;

        public UISyncObservableCollection()
        {
        }

        public UISyncObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable)
        {
        }

        [NotNull]
        private IUISynchronize InternalUISynchronize
        {
            get
            {
                if (_synchronize != null) return _synchronize;
                _synchronize = UiSynchronize.Synchronize ?? new DummySync();

                return _synchronize;
            }
        }

        #region Methods

        /// <summary>
        ///     The on collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            InternalUISynchronize.Invoke(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnPropertyChanged([NotNull] PropertyChangedEventArgs e)
        {
            InternalUISynchronize.Invoke(() => base.OnPropertyChanged(e));
        }

        #endregion

        public void AddRange([NotNull] IEnumerable<TType> enumerable)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null, "enumerable");

            foreach (var item in enumerable) Add(item);
        }
    }
}