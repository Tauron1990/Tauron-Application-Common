using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public sealed class PropertyIssue
    {
        public PropertyIssue([NotNull] string propertyName, [CanBeNull] object value, [NotNull] string message)
        {
            PropertyName = Argument.NotNull(propertyName, nameof(propertyName));
            Value = value;
            Message = Argument.NotNull(message, nameof(message));
        }

        [NotNull]
        public string PropertyName { get; }

        [CanBeNull]
        public object Value { get; }

        [NotNull]
        public string Message { get; }

        public override string ToString() => Message;
    }
}