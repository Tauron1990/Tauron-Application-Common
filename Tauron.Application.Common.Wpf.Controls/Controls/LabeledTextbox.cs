using System.Windows;
using System.Windows.Controls;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    [PublicAPI]
    public class LabeledTextbox : TextBox
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof (string), typeof (LabeledTextbox), new PropertyMetadata(default(string)));

        [CanBeNull]
        public string LabelText
        {
            get { return (string) GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        static LabeledTextbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledTextbox), new PropertyMetadata(typeof(LabeledTextbox)));
        }
    }
}
