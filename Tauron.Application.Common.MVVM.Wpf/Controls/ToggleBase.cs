using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    [PublicAPI]
    public abstract class ToggleSwitchSelectionBase : ItemsControl
    {
        public static readonly DependencyProperty ActiveItemProperty = 
            DependencyProperty.Register("ActiveItem", typeof(object), typeof(ToggleSwitchSelectionBase), new UIPropertyMetadata(null, OnPropertyChanged));

        public object ActiveItem
        {
            get => GetValue(ActiveItemProperty);
            set => SetValue(ActiveItemProperty, value);
        }

        protected virtual void SelectionChagend(object item) { }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ToggleSwitchSelectionBase) d).SelectionChagend(e.NewValue);
    }

    public abstract class ToggleSwitchBase<TToggle> : ToggleSwitchSelectionBase
        where TToggle : DependencyObject, IToggle, new()
    {
        private readonly Dictionary<int, TToggle> _controls;

        protected ToggleSwitchBase() => _controls = new Dictionary<int, TToggle>();

        protected IToggle Active { get; private set; }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var toggle = (TToggle) element;
            foreach (var item2 in _controls)
                if (ReferenceEquals(item, toggle))
                {
                    _controls.Remove(item2.Key);
                    break;
                }

            ClearItem(toggle, item);
            toggle.SetHeader(null);
            toggle.Switched -= ToggleSwitched;
        }

        protected virtual void ClearItem(TToggle toggle, object item) { }

        protected virtual TToggle CreateContainer() => new TToggle();

        protected override DependencyObject GetContainerForItemOverride() => CreateContainer();

        protected abstract object GetHeader(object item);

        protected abstract object GetItem(TToggle toggle);

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TToggle || item is TextBlock;

        protected virtual void ItemActivateted(TToggle toggle) { }

        protected virtual void ItemDeactivatet(TToggle toggle) { }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var toggle = (TToggle) element;
            PrepateItem(toggle, item);
            _controls[Items.IndexOf(item)] = toggle;
            if (item is IHeaderProvider provider)
                toggle.SetHeader(provider.Header);
            else
            {
                var header = GetHeader(item);
                toggle.SetHeader(header ?? item.ToString());
            }

            toggle.Switched += ToggleSwitched;
        }

        protected virtual void PrepateItem(TToggle toggle, object item) { }

        protected override void SelectionChagend(object item)
        {
            if (item == null && Active != null)
            {
                Active.Switch();
                return;
            }

            var control = _controls[Items.IndexOf(item)];
            if (!Equals(control, Active)) control.Switch();
        }

        private void ToggleSwitched(IToggle sender, bool stade)
        {
            if (stade)
            {
                Active?.Switch();
                Active = sender;

                var item = GetItem((TToggle) Active);
                if (ActiveItem != null && ActiveItem != item) ActiveItem = item;

                ItemActivateted((TToggle) sender);
            }
            else
            {
                ItemDeactivatet((TToggle) Active);
                Active = null;
            }
        }
    }
}