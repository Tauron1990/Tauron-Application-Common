using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public sealed class ValidatorContext
    {
        [NotNull]
        public ModelBase Model { get; private set; }

        [NotNull]
        public ObservableProperty Property { get; internal set; }

        [NotNull]
        public Type ModelType { get { return Model.GetType(); } }

        [NotNull]
        public IDictionary<object, object> Items { get; private set; }

        public ValidatorContext([NotNull] ModelBase model)
        {
            if (model == null) throw new ArgumentNullException("model");
            Model = model;
            Items = new Dictionary<object, object>();
        }
    }
}