﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public interface IContainer : IDisposable, IServiceProvider
    {
        object BuildUp(ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters);

        object BuildUp(object toBuild, ErrorTracer errorTracer, params BuildParameter[] parameters);

        object BuildUp(Type type, ErrorTracer errorTracer, BuildParameter[] buildParameters, params object[] constructorArguments);

        ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional);

        ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer);

        [NotNull]
        IEnumerable<ExportMetadata> FindExports([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer);

        [NotNull]
        ExportMetadata FindExport([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer, bool isOptional, int level);

        [NotNull]
        IEnumerable<ExportMetadata> FindExports([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer, int level);

        void Register(IExport exportType, int level);

        void Register(ExportResolver exportResolver);

        void Register(IContainerExtension extension);
    }
}