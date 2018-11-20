using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    public class EditableTextBlockAdorner : Adorner
    {
        public EditableTextBlockAdorner([NotNull] EditableTextBlock adornedElement)
            : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            _textBox = new TextBox();
            _textBlock = adornedElement;

            var binding = new Binding("Text") {Source = adornedElement};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.AcceptsReturn = true;
            _textBox.MaxLength = adornedElement.MaxLength;
            _textBox.KeyUp += TextBoxKeyUpEventHandler;

            _collection.Add(_textBox);
        }

        protected override int VisualChildrenCount => _collection.Count;

        private readonly VisualCollection _collection;

        private readonly TextBlock _textBlock;

        private readonly TextBox _textBox;

        public event KeyEventHandler TextBoxKeyUp
        {
            add => _textBox.KeyUp += value;
            remove => _textBox.KeyUp -= value;
        }

        public event RoutedEventHandler TextBoxLostFocus
        {
            add => _textBox.LostFocus += value;
            remove => _textBox.LostFocus -= value;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBox.Arrange(new Rect(0, 0, _textBlock.DesiredSize.Width + 50, _textBlock.DesiredSize.Height * 1.5));
            _textBox.Focus();
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _collection[index];

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(null, new Pen {Brush = Brushes.Gold, Thickness = 2}, new Rect(0, 0, _textBlock.DesiredSize.Width + 50, _textBlock.DesiredSize.Height * 1.5));
        }

        private void TextBoxKeyUpEventHandler([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            _textBox.Text = _textBox.Text.Replace("\r\n", string.Empty);
            var expression = _textBox.GetBindingExpression(TextBox.TextProperty);
            expression?.UpdateSource();
        }
    }
}