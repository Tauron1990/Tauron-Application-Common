using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy.Steps;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    [PublicAPI]
    public abstract class Injectorbase<TMember> : MemberInjector
    {
        protected Injectorbase([NotNull] IMetadataFactory metadataFactory, [NotNull] TMember member, [NotNull] IResolverExtension[] resolverExtensions)
        {
            Member = Argument.NotNull(member, nameof(member));
            // ReSharper disable once VirtualMemberCallInConstructor
            InjectorContext = new InjectorContext(Argument.NotNull(metadataFactory, nameof(metadataFactory)), member, MemberType, Argument.NotNull(resolverExtensions, nameof(resolverExtensions)));
        }

        [NotNull]
// ReSharper disable once MemberCanBePrivate.Global
        protected InjectorContext InjectorContext { get; private set; }

        [NotNull]
        protected TMember Member { get; }

        [NotNull]
        protected abstract Type MemberType { get; }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual StepId InitializeMachine([NotNull] out ResolverFactory solidMachine)
        {
            solidMachine = ResolverFactory.DefaultResolverFactory;

            return ResolverFactory.StartId;
        }

        public override void Inject(object target, IContainer container, ImportMetadata metadata,
            IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters, FactoryCacheEntry entry)
        {
            try
            {
                errorTracer.Phase = "Creating Resolver for " + target.GetType().Name + "(" + metadata + ")";

                InjectorContext.Tracer = errorTracer;
                InjectorContext.Metadata = metadata;
                InjectorContext.ImportInterceptor = interceptor;
                InjectorContext.BuildParameters = parameters;
                InjectorContext.Container = container;
                InjectorContext.Target = target;

                var start = InitializeMachine(out var fac);

                InjectorContext.Machine = fac;

                fac.Begin(start, InjectorContext);

                if (InjectorContext.Resolver == null) throw new InvalidOperationException("No Resolver Created");

                var value = InjectorContext.Resolver;

                if (errorTracer.Exceptional) return;

                Inject(target, value.Create(errorTracer));
                
                if(entry == null) return;

                var injectAction = GetInjectAction();
                entry.BuildActions.Add(c => injectAction(c.Target, value.Create(c.ErrorTracer)));
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }

        protected abstract Action<object, object> GetInjectAction();
        protected abstract void Inject([NotNull] object target, [CanBeNull] object value);
    }
}