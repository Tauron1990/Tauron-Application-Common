using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public class ModelRule : IEquatable<ModelRule>
    {
        public ModelRule([NotNull] Func<object, ValidatorContext, ValidatorResult> validator, string id)
        {
            Id = id;
            Validator = Argument.NotNull(validator, nameof(validator));
        }

        protected ModelRule(string id) => Id = Id;

        [CanBeNull]
        // ReSharper disable once MemberCanBePrivate.Global
        protected Func<object, ValidatorContext, ValidatorResult> Validator { get; set; }

        //[CanBeNull]
        //public Func<string> Message { get; set; }

        [CanBeNull]
        public string Id { get; }

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

        public virtual ValidatorResult IsValidValue([CanBeNull] object value, [NotNull] ValidatorContext context) => Validator == null ? CreateResult() : Validator(value, context);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModelRule) obj);
        }

        [NotNull]
        protected string FindResource(string name) => ResourceManagerProvider.FindResource(name, null) ?? string.Empty;

        protected  static ValidatorResult CreateResult() => new ValidatorResult(string.Empty, true);
        protected static ValidatorResult CreateResult(string message) => new ValidatorResult(message, false);
    }
}