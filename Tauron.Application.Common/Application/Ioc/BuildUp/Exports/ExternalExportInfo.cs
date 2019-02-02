using System;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public sealed class ExternalExportInfo
    {
        private readonly Func<IBuildContext, IRightable> _createInstance;

        public ExternalExportInfo(bool external, bool handlesLiftime, bool canUseBuildup, bool handlesDispose, Func<IBuildContext, IRightable> createInstance,
            string extenalComponentName)
        {
            External = external;
            HandlesLiftime = handlesLiftime;
            CanUseBuildup = canUseBuildup;
            HandlesDispose = handlesDispose;
            ExtenalComponentName = extenalComponentName;
            _createInstance = createInstance;
        }

        public IRightable Create(IBuildContext context)//, ProxyGenerator service)
            => _createInstance(context);

        public bool CanUseBuildup { get; private set; }

        public string ExtenalComponentName { get; private set; }

        public bool External { get; private set; }

        public bool HandlesDispose { get; private set; }

        public bool HandlesLiftime { get; private set; }
    }
}