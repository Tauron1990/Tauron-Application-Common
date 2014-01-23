#region Usings

using System;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class MappingEntryBase<TContext> : MappingEntry<TContext>
        where TContext : IOrginalContextProvider
    {
        private readonly Func<object, object> _accessor;
        private readonly Action<object, object> _setter;

        [CanBeNull]
        protected MemberInfo TargetMember { get; private set; }

        [CanBeNull]
        protected Type MemberType { get; private set; }

        protected MappingEntryBase([CanBeNull] string membername, [CanBeNull] Type targetType)
        {
            if(targetType == null) return;

            MemberInfo mem = targetType.GetMember(membername, MemberTypes.Field | MemberTypes.Property,
                                                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                       .FirstOrDefault();

            if (mem == null) return;

            TargetMember = mem;

            var prop = mem as PropertyInfo;
            if (prop != null)
            {
                MemberType = prop.PropertyType;

                if (prop.CanRead) _accessor = prop.GetValue;
                if (prop.CanWrite) _setter = prop.SetValue;
                return;
            }

            var fld = mem as FieldInfo;
            if (fld == null) return;

            MemberType = fld.FieldType;
            _accessor = fld.GetValue;
            _setter = fld.SetValue;
        }

        protected void SetValue([NotNull] object target, [CanBeNull] object value)
        {
            _setter(target, value);
        }

        [NotNull]
        protected object GetValue([NotNull] object target)
        {
            return _accessor(target);
        }

        public override Exception VerifyError()
        {
            if(_accessor == null || _setter == null)
                return new SerializerElementNullException("Member");
            return null;
        }

        public override void Progress(object target, TContext context, SerializerMode mode)
        {
            switch (mode)
            {
                case SerializerMode.Deserialize:
                    Deserialize(target, context);
                    break;
                case SerializerMode.Serialize:
                    Serialize(target, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        protected abstract void Deserialize([NotNull] object target, [NotNull] TContext context);
        protected abstract void Serialize([NotNull] object target, [NotNull] TContext context);
    }
}
