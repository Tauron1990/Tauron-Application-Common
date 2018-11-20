using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    public sealed class TopicChangedEvntArgs : EventArgs
    {
        public TopicChangedEvntArgs([CanBeNull] object topic) => Topic = topic;

        [CanBeNull]
        public object Topic { get; }

    }

    public sealed class ToggleContentButton : ToggleButton, IToggle
    {
        [CanBeNull]
        public object Topic { get; set; }

        public event Action<IToggle, bool> Switched;

        public void SetHeader([NotNull] object header) => Content = header;

        public void Switch() => IsChecked = IsChecked != true;

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            Switched?.Invoke(this, true);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            Switched?.Invoke(this, false);
        }

    }

    public class ToggleButtonList : ToggleSwitchBase<ToggleContentButton>
    {
        public static readonly DependencyProperty TopicProperty = 
            DependencyProperty.RegisterAttached("Topic", typeof(object), typeof(ToggleButtonList), new UIPropertyMetadata(null));

        static ToggleButtonList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonList),new FrameworkPropertyMetadata(typeof(ToggleButtonList)));

        public event EventHandler<TopicChangedEvntArgs> TopicChangedEvent;
        public event EventHandler TopicDeactivatet;

        [CanBeNull]
        public static object GetTopic([NotNull] DependencyObject obj) => obj.GetValue(TopicProperty);

        public static void SetTopic([NotNull] DependencyObject obj, [NotNull] object value) => obj.SetValue(TopicProperty, value);

        protected override void ClearItem([NotNull] ToggleContentButton toggle, [NotNull] object item) => toggle.Topic = null;

        [NotNull]
        protected override object GetHeader([NotNull] object item) => item;

        [NotNull]
        protected override object GetItem([NotNull] ToggleContentButton toggle) => toggle.Content;

        protected override void ItemActivateted([NotNull] ToggleContentButton toggle)
        {
            var handler = TopicChangedEvent;
            handler?.Invoke(this, new TopicChangedEvntArgs(toggle.Topic));
        }

        protected override void ItemDeactivatet([NotNull] ToggleContentButton toggle)
        {
            var handler = TopicDeactivatet;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected override void PrepateItem([NotNull] ToggleContentButton toggle, [NotNull] object item)
        {
            var topic = GetTopic(toggle);
            if (topic == null)
            {
                if (item is DependencyObject itemdo)
                    topic = GetTopic(itemdo);
            }

            toggle.Topic = topic;
        }
    }
}