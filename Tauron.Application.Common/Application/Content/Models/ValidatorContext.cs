using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public sealed class ValidatorContext
    {
        public ValidatorContext([NotNull] ModelBase model)
        {
            Model = Argument.NotNull(model, nameof(model));
            Items = new Dictionary<object, object>();
        }

        [NotNull]
        public ModelBase Model { get; }

        [CanBeNull]
        public ObservableProperty Property { get; internal set; }

        [NotNull]
        public Type ModelType => Model.GetType();

        [NotNull]
        public IDictionary<object, object> Items { get; }
    }
}