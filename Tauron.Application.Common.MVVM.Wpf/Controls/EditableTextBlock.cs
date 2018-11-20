using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    [PublicAPI]
    public class EditableTextBlock : TextBlock
    {
        private EditableTextBlockAdorner _adorner;

        public static readonly DependencyProperty IsInEditModeProperty
            = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false, IsInEditModeUpdate));

        public static readonly DependencyProperty MaxLengthProperty 
            = DependencyProperty.Register("MaxLength", typeof(int), typeof(EditableTextBlock), new UIPropertyMetadata(0));

        public bool IsInEditMode
        {
            get => (bool) GetValue(IsInEditModeProperty);
            set => SetValue(IsInEditModeProperty, value);
        }

        public int MaxLength
        {
            get => (int) GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) IsInEditMode = true;
            else if (e.ClickCount == 2) IsInEditMode = true;
        }

        private static void IsInEditModeUpdate([NotNull] DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = obj as EditableTextBlock;
            if (null == textBlock) return;

            // Get the adorner layer of the uielement (here TextBlock)
            var layer = AdornerLayer.GetAdornerLayer(textBlock);

            // If the IsInEditMode set to true means the user has enabled the edit mode then
            // add the adorner to the adorner layer of the TextBlock.
            if (textBlock.IsInEditMode)
            {
                if (null == textBlock._adorner)
                {
                    textBlock._adorner = new EditableTextBlockAdorner(textBlock);

                    // Events wired to exit edit mode when the user presses Enter key or leaves the control.
                    textBlock._adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
                    textBlock._adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
                }

                layer.Add(textBlock._adorner);
            }
            else
            {
                // Remove the adorner from the adorner layer.
                var adorners = layer.GetAdorners(textBlock);
                if (adorners != null)
                {
                    foreach (var adorner in adorners.OfType<EditableTextBlockAdorner>())
                        layer.Remove(adorner);
                }

                // Update the textblock's text binding.
                var expression = textBlock.GetBindingExpression(TextProperty);
                expression?.UpdateTarget();
            }
        }

        private void TextBoxKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                IsInEditMode = false;
        }

        private void TextBoxLostFocus([NotNull] object sender, [NotNull] RoutedEventArgs e) => IsInEditMode = false;
    }
}