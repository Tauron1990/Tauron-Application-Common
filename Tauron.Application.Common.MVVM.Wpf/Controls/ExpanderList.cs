using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    public sealed class ToggleExpander : Expander, IToggle
    {
        public event Action<IToggle, bool> Switched;

        public void SetHeader([NotNull] object header) => Header = Argument.NotNull(header, nameof(header));

        public void Switch() => IsExpanded = !IsExpanded;

        protected override void OnCollapsed()
        {
            base.OnCollapsed();
            Switched?.Invoke(this, false);
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();
            Switched?.Invoke(this, true);
        }

    }

    [PublicAPI]
    public class ExpanderList : ToggleSwitchBase<ToggleExpander>
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
            "Header",
            typeof(object),
            typeof(
                ExpanderList),
            new FrameworkPropertyMetadata
            (
                null,
                FrameworkPropertyMetadataOptions
                    .AffectsRender |
                FrameworkPropertyMetadataOptions
                    .AffectsMeasure));

        static ExpanderList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderList), new FrameworkPropertyMetadata(typeof(ExpanderList)));
        }

        [NotNull]
        public static object GetHeader([NotNull] DependencyObject obj) => obj.GetValue(HeaderProperty);

        public static void SetHeader([NotNull] DependencyObject obj, [NotNull] object value) => obj.SetValue(HeaderProperty, value);

        [CanBeNull]
        protected override object GetHeader([NotNull] object item) => !(item is DependencyObject obj) ? null : GetHeader(obj);

        protected override object GetItem(ToggleExpander toggle) => toggle.Content;
    }
}