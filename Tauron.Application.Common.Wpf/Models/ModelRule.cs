using System;
using System.Runtime.CompilerServices;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public class ModelRule : IEquatable<ModelRule>
    {
        public bool Equals([CanBeNull] ModelRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Id == null
                ? RuntimeHelpers.Equals(this, other) : string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : RuntimeHelpers.GetHashCode(this));
        }

        public static bool operator ==(ModelRule left, ModelRule right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelRule left, ModelRule right)
        {
            return !Equals(left, right);
        }

        [CanBeNull]
        // ReSharper disable once MemberCanBePrivate.Global
        protected Func<object, ValidatorContext, bool> Validator { get; set; }

        [NotNull]
        public Func<string> Message { get; set; }

        [CanBeNull]
        public string Id { get; set; }

        public virtual bool IsValidValue([CanBeNull] object obj, [NotNull] ValidatorContext context)
        {
            return Validator == null || Validator(obj, context);
        }

        public ModelRule([NotNull] Func<object, ValidatorContext, bool> validator)
        {
            if (validator == null) throw new ArgumentNullException("validator");
            Validator = validator;
        }

        protected ModelRule()
        {
            
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelRule) obj);
        }
    }
}
