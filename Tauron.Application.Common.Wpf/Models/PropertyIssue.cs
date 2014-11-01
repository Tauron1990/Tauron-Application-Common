using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public sealed class PropertyIssue
    {
        [NotNull]
        public string PropertyName { get; private set; }

        [CanBeNull]
        public object Value { get; private set; }

        [NotNull]
        public string Message { get; private set; }

        public PropertyIssue([NotNull] string propertyName, [CanBeNull] object value, [NotNull] string message)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (message == null) throw new ArgumentNullException("message");

            PropertyName = propertyName;
            Value = value;
            Message = message;
        }
    }
}