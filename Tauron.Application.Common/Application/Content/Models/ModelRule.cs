using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public class ModelRule : IEquatable<ModelRule>
    {
        public ModelRule([NotNull] Func<object, ValidatorContext, bool> validator) => Validator = Argument.NotNull(validator, nameof(validator));

        protected ModelRule()
        {
        }

        [CanBeNull]
        // ReSharper disable once MemberCanBePrivate.Global
        protected Func<object, ValidatorContext, bool> Validator { get; set; }

        [CanBeNull]
        public Func<string> Message { get; set; }

        [CanBeNull]
        public string Id { get; set; }

        public bool Equals(ModelRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Id == null
                ? RuntimeHelpers.Equals(this, other)
                : string.Equals(Id, other.Id);
        }

        public override int GetHashCode() => Id?.GetHashCode() ?? RuntimeHelpers.GetHashCode(this);

        public static bool operator ==(ModelRule left, ModelRule right) => Equals(left, right);

        public static bool operator !=(ModelRule left, ModelRule right) => !Equals(left, right);

        public virtual bool IsValidValue([CanBeNull] object obj, [NotNull] ValidatorContext context) => Validator == null || Validator(obj, context);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModelRule) obj);
        }
    }
}