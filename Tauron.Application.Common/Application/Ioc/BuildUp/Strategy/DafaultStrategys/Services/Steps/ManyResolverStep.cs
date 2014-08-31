using System;
using System.Collections.Generic;
using Tauron.Application.SimpleWorkflow;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public abstract class ManyResolverStep : InjectorStep
    {
        private ExportEnumeratorHelper _enumeratorHelper;
        private List<IResolver> _resolvers;
        private Type _listType;

        public override StepId OnExecute(InjectorContext context)
        {
            _listType = context.ReflectionContext.CurrentType;
            context.ReflectionContext.CurrentType = GetCurrentType(context.ReflectionContext);

            var findAllExports = context.ReflectionContext.FindAllExports();
            if (findAllExports == null) return StepId.Invalid;

            _resolvers = new List<IResolver>();
            _enumeratorHelper = new ExportEnumeratorHelper(findAllExports.GetEnumerator(), context.ReflectionContext);
            return StepId.Loop;
        }

        [NotNull]
        protected abstract Type GetCurrentType([NotNull] ReflectionContext context);

        public override StepId NextElement(InjectorContext context)
        {
            if (context.Resolver != null)
                _resolvers.Add(context.Resolver);
            _enumeratorHelper.MoveNext();
            return _enumeratorHelper.NextId;
        }

        [NotNull]
        protected abstract IResolver CreateResolver([NotNull] IEnumerable<IResolver> resolvers, [NotNull] Type listType);

        public override void OnExecuteFinish(InjectorContext context)
        {
            context.Resolver = CreateResolver(_resolvers, _listType);
            _resolvers = null;
            _listType = null;
            _enumeratorHelper = null;
        }
    }
}