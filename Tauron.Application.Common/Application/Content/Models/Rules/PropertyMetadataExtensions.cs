using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public static class PropertyMetadataExtensions
    {
        private static Dictionary<ObservablePropertyMetadata, ModelRule[]> _rules = new Dictionary<ObservablePropertyMetadata, ModelRule[]>();

        [NotNull]
        public static ObservablePropertyMetadata SetValidationRules(this ObservablePropertyMetadata metadata, [NotNull] params ModelRule[] rules)
        {
            Argument.NotNull(metadata, nameof(metadata));

            PropertyInfo info = metadata.OwnerType.GetProperty(metadata.PropertyName);
            if (info == null) return metadata;

            var arules = info.GetCustomAttributes(true).OfType<ModelRule>();

            if (rules.Length != 0)
                arules = rules.Concat(Argument.NotNull(rules, nameof(rules)));

           var modelRules = arules.ToArray();

           _rules[metadata] = modelRules;

            return metadata;
        }

        [CanBeNull]
        public static ModelRule[] GetModelRules(this ObservablePropertyMetadata metadata) 
            => _rules.TryGetValue(Argument.NotNull(metadata, nameof(metadata)), out var rules) ? rules : Array.Empty<ModelRule>();
    }
}