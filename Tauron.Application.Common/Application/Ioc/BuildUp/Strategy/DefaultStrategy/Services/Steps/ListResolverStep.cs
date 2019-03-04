using System;
using System.Collections.Generic;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy.Steps
{
    public class ListResolverStep : ManyResolverStep
    {
        public override StepId Id => StepIds.ListResolver;

        protected override Type GetCurrentType(ReflectionContext context) => context.CurrentType.GenericTypeArguments[0];

        protected override IResolver CreateResolver(IEnumerable<IResolver> resolvers, Type listType) => new ListResolver(resolvers, listType);
    }
}