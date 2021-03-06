﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application
{
    //[DebuggerNonUserCode]
    [PublicAPI]
    [Serializable]
    public class UIObservableCollection<TType> : ObservableCollection<TType>
    {
        private bool _isBlocked;

        private IUISynchronize _synchronize;

        public UIObservableCollection(){}

        public UIObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable){}

        [NotNull]
        protected IUISynchronize InternalUISynchronize
        {
            get
            {
                if (_synchronize != null) return _synchronize;
                _synchronize = UiSynchronize.Synchronize ?? new DummySync();

                return _synchronize;
            }
        }

        public void AddRange([NotNull] IEnumerable<TType> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            foreach (var item in enumerable) Add(item);
        }

        public IDisposable BlockChangedMessages() => new DispoableBlocker(this);

        private class DispoableBlocker : IDisposable
        {
            private readonly UIObservableCollection<TType> _collection;

            public DispoableBlocker(UIObservableCollection<TType> collection)
            {
                _collection = collection;
                _collection._isBlocked = true;
            }

            public void Dispose()
            {
                _collection._isBlocked = false;
                _collection.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private class DummySync : IUISynchronize
        {
            public Task BeginInvoke(Action action)
            {
                action();
// ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }

            public Task<TResult> BeginInvoke<TResult>(Func<TResult> action) => null;

            public void Invoke(Action action)
            {
                action();
            }

            public TReturn Invoke<TReturn>(Func<TReturn> action) => action();

            public bool CheckAccess { get; } = true;
        }
        
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_isBlocked) return;
            if (InternalUISynchronize.CheckAccess)
                base.OnCollectionChanged(e);
            InternalUISynchronize.Invoke(() => base.OnCollectionChanged(e));
        }
        
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_isBlocked) return;
            if (InternalUISynchronize.CheckAccess) base.OnPropertyChanged(e);
            else InternalUISynchronize.Invoke(() => base.OnPropertyChanged(e));
        }
    }
}