using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps;
using Tauron.Application.SimpleWorkflow;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public abstract class Injectorbase<TMember> : MemberInjector
    {
        [NotNull]
// ReSharper disable once MemberCanBePrivate.Global
        protected InjectorContext InjectorContext { get; private set; }

        private TMember _member;

        [NotNull]
        protected TMember Member
        {
            get
            {
// ReSharper disable once CompareNonConstrainedGenericWithNull
                Contract.Ensures(Contract.Result<TMember>() != null);

                return _member;
            }
        }

        [NotNull]
        protected abstract Type MemberType { get; }

        protected Injectorbase([NotNull] IMetadataFactory metadataFactory, [NotNull] TMember member, IResolverExtension[] resolverExtensions)
        {
            Contract.Requires<ArgumentNullException>(metadataFactory != null, "metadataFactory");

            _member = member;

// ReSharper disable once DoNotCallOverridableMethodsInConstructor
            InjectorContext = new InjectorContext(metadataFactory, member, MemberType, resolverExtensions);
        }

        protected virtual StepId InitializeMachine([NotNull] out ResolverFactory solidMachine)
        {
            solidMachine = ResolverFactory.DefaultResolverFactory;

            return ResolverFactory.StartId;
        }

        public override void Inject(object target, IContainer container, ImportMetadata metadata,
                                    IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters)
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

                ResolverFactory fac;

                var start = InitializeMachine(out fac);

                InjectorContext.Machine = fac;

                fac.Beginn(start, InjectorContext);

                if (InjectorContext.Resolver == null) throw new InvalidOperationException("No Resolver Created");

                var value = InjectorContext.Resolver.Create(errorTracer);

                if (errorTracer.Exceptional) return;

                Inject(target, value);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }
        protected abstract void Inject([NotNull] object target, [CanBeNull] object value);
    }
}