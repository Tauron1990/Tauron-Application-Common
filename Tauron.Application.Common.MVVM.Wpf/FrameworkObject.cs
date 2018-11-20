#region

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The framework object.</summary>
    [DebuggerStepThrough]
    [PublicAPI]
    public sealed class FrameworkObject : IWeakReference
    {
        public FrameworkObject([CanBeNull] object obj, bool isWeak = true)
        {
            var fe = obj as FrameworkElement;
            var fce = obj as FrameworkContentElement;

            _isFe = fe != null;
            _isFce = fce != null;
            IsValid = _isFce || _isFe;

            // ReSharper disable AssignNullToNotNullAttribute
            if (_isFe) _fe = new ElementReference<FrameworkElement>(fe, isWeak);
            else if (_isFce) _fce = new ElementReference<FrameworkContentElement>(fce, isWeak);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        bool IWeakReference.IsAlive
        {
            get
            {
                if (_isFe) return _fe.IsAlive;

                return _isFce && _fce.IsAlive;
            }
        }

        [DebuggerStepThrough]
        private class ElementReference<TReference> : IWeakReference
            where TReference : class
        {
            public ElementReference([NotNull] TReference reference, bool isWeak)
            {
                if (isWeak) _weakRef = new WeakReference<TReference>(Argument.NotNull(reference, nameof(reference)));
                else _reference = reference;
            }

            private readonly TReference _reference;

            private readonly WeakReference<TReference> _weakRef;

            [NotNull]
            public TReference Target => _weakRef != null ? Argument.CheckResult(_weakRef.TypedTarget(), "Weak Element Was Null") : _reference;

            public bool IsAlive => _weakRef == null || _weakRef.IsAlive();
        }

        private readonly ElementReference<FrameworkContentElement> _fce;

        private readonly ElementReference<FrameworkElement> _fe;

        private readonly bool _isFce;

        private readonly bool _isFe;

        public event DependencyPropertyChangedEventHandler DataContextChanged
        {
            add
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContextChanged += value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContextChanged += value;
            }

            remove
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContextChanged -= value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContextChanged -= value;
            }
        }

        public event RoutedEventHandler LoadedEvent
        {
            add
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.Loaded += value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.Loaded += value;
            }

            remove
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.Loaded -= value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.Loaded -= value;
            }
        }

        [CanBeNull]
        public object DataContext
        {
            get
            {
                if (!IsValid) return null;

                if (TryGetFrameworkElement(out var fe)) return fe.DataContext;
                return TryGetFrameworkContentElement(out var fce) ? fce.DataContext : null;
            }

            set
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContext = value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContext = value;
            }
        }

        public bool IsValid { get; }

        [CanBeNull]
        public DependencyObject Original
        {
            get
            {
                if (_isFe) return _fe.Target;
                return _isFce ? _fce.Target : null;
            }
        }

        [CanBeNull]
        public DependencyObject Parent
        {
            get
            {
                if (!IsValid) return null;

                if (TryGetFrameworkElement(out var fe)) return fe.Parent;
                return TryGetFrameworkContentElement(out var fce) ? fce.Parent : null;
            }
        }

        [CanBeNull]
        public DependencyObject VisualParent
        {
            get
            {
                if (!IsValid) return null;

                return TryGetFrameworkElement(out var fe) ? VisualTreeHelper.GetParent(fe) : null;
            }
        }

        public bool TryGetFrameworkContentElement(out FrameworkContentElement contentElement)
        {
            contentElement = _isFce ? _fce.Target : null;

            return contentElement != null;
        }

        public bool TryGetFrameworkElement(out FrameworkElement frameworkElement)
        {
            frameworkElement = _isFe ? _fe.Target : null;

            return frameworkElement != null;
        }
    }
}