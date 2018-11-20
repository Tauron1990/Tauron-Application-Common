using System;
using System.Collections.Generic;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ArrayResolverStep : ManyResolverStep
    {
        public override StepId Id => StepIds.ArrayResolver;

        protected override Type GetCurrentType(ReflectionContext context) => Argument.CheckResult(context.CurrentType.GetElementType(), $"Type Must Be a Array {context.CurrentType}");

        protected override IResolver CreateResolver(IEnumerable<IResolver> resolvers, Type listType) => new ArrayResolver(resolvers, listType.GetElementType());
    }
}