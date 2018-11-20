using System;
using System.Collections.Generic;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    internal static class InjectorBaseConstants
    {
        internal static readonly Type Lazy = typeof(Lazy<>);

        internal static readonly Type LazyWithMetadata = typeof(Lazy<,>);

        internal static readonly Type List = typeof(List<>);

    }
}