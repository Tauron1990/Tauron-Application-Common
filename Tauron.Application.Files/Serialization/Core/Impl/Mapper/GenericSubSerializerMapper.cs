using System;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    public abstract class GenericSubSerializerMapper<TContext> : MappingEntryBase<TContext>
        where TContext : class, IOrginalContextProvider
    {
        private readonly ISubSerializer _serializer;

        protected GenericSubSerializerMapper([CanBeNull] string membername, [NotNull] Type targetType,
                                             [CanBeNull] ISubSerializer serializer)
            : base(membername, targetType)
        {
            _serializer = serializer;
        }

        protected override void Deserialize(object target, TContext context)
        {
            SetValue(target, _serializer.Deserialize(GetRealContext(context, SerializerMode.Deserialize)));
        }

        protected override void Serialize(object target, TContext context)
        {
            _serializer.Serialize(GetRealContext(context, SerializerMode.Serialize), GetValue(target));
        }

        public override Exception VerifyError()
        {
            Exception e = base.VerifyError();

            if(_serializer == null)
                e = new SerializerElementNullException("The Serializer does not Support the SupSerializer Interface");

            return e;
        }

        [NotNull]
        protected virtual SerializationContext GetRealContext([NotNull] TContext origial, SerializerMode mode)
        {
            return origial.Original;
        }

        protected virtual void PostProgressing([NotNull] SerializationContext context)
        {
        }
    }
}
