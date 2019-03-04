using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class ExternalImportInterceptorPolicy : IPolicy
    {
        public ExternalImportInterceptorPolicy() => Interceptors = new List<IImportInterceptor>();

        [NotNull]
        public List<IImportInterceptor> Interceptors { get; }
    }
}