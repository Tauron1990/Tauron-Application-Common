using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    [PublicAPI]
    public abstract class SerializerBase<TContext> : ISubSerializer
        where TContext : class, IOrginalContextProvider 
    {
        private readonly ObjectBuilder _builder;
        private readonly SimpleMapper<TContext> _mapper;
        private readonly ContextMode _contextMode;

        protected SerializerBase([NotNull] ObjectBuilder builder, [NotNull] SimpleMapper<TContext> mapper, ContextMode contextMode)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (mapper == null) throw new ArgumentNullException("mapper");

            _builder = builder;
            _mapper = mapper;
            _contextMode = contextMode;
        }

        public virtual AggregateException Errors
        {
            get
            {
                var errors = new List<Exception>();
                Exception e = _builder.Verfiy();

                if (e != null) errors.Add(e);

                foreach (var mappingEntry in _mapper.Entries)
                {
                    e = mappingEntry.VerifyError();
                    if (e != null) errors.Add(e);
                }

                return errors.Count != 0 ? new AggregateException(errors) : null;
            }
        }

        public virtual void Serialize(IStreamSource target, object graph)
        {
            Progress(graph, target, SerializerMode.Serialize);
        }

        public virtual object Deserialize(IStreamSource target)
        {
            object garph = BuildObject();
            
            Progress(garph, target, SerializerMode.Deserialize);

            return garph;
        }

        public virtual void Deserialize(IStreamSource targetStream, object target)
        {
            Progress(target, targetStream, SerializerMode.Deserialize);
        }

        [NotNull]
        protected object BuildObject()
        {
// ReSharper disable once PossibleNullReferenceException
            return _builder.BuilderFunc(_builder.CustomObject);
        }

        private void Progress([NotNull] object graph, [NotNull] IStreamSource target, SerializerMode mode)
        {
            var context = BuildContext(new SerializationContext(_contextMode, target, mode));
            Progress(graph, context, mode);
        }

        public void Progress([NotNull] object graph, [NotNull] TContext context, SerializerMode mode)
        {
            foreach (var mappingEntry in _mapper.Entries) mappingEntry.Progress(graph, context, mode);

            CleanUp(context);
        }

        [NotNull]
        public abstract TContext BuildContext([NotNull] SerializationContext context);
        public abstract void CleanUp([NotNull] TContext context);
        void ISubSerializer.Serialize(SerializationContext target, object graph)
        {
            TContext context = BuildContext(target);

            foreach (var mappingEntry in _mapper.Entries)
            {
                mappingEntry.Progress(graph, context, SerializerMode.Serialize);
            }
        }

        object ISubSerializer.Deserialize(SerializationContext target)
        {
            object obj = BuildObject();
            TContext context = BuildContext(target);

            foreach (var mappingEntry in _mapper.Entries)
            {
                mappingEntry.Progress(obj, context, SerializerMode.Deserialize);
            }

            return obj;
        }
    }
}
