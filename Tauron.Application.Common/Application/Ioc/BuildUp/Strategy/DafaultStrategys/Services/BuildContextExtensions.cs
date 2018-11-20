using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public static class BuildContextExtensions
    {
        public static bool CanCache([NotNull] this IBuildContext context) => 
            Argument.NotNull(context, nameof(context)).Mode != BuildMode.BuildUpObject || context.Target != null
                                             || context.Metadata.Export.ExternalInfo.External
                                             && !context.Metadata.Export.ExternalInfo.HandlesLiftime;

        public static bool CanHandleLiftime([NotNull] this IBuildContext context) => !Argument.NotNull(context, nameof(context)).Metadata.Export.ExternalInfo.External
                                                                                     && !context.Metadata.Export.ExternalInfo.HandlesLiftime;

        public static bool CanUseBuildUp([NotNull] this IBuildContext context) => 
            !Argument.NotNull(context, nameof(context)).Metadata.Export.ExternalInfo.External || context.Metadata.Export.ExternalInfo.CanUseBuildup;

        public static bool IsBuildExport([NotNull] this IBuildContext context) => Argument.NotNull(context, nameof(context)).Mode != BuildMode.BuildUpObject;

        public static bool IsResolving([NotNull] this IBuildContext context) => Argument.NotNull(context, nameof(context)).Mode == BuildMode.Resolve;

        public static bool UseInternalInstantiation([NotNull] this IBuildContext context) => !Argument.NotNull(context, nameof(context)).Metadata.Export.ExternalInfo.External;
    }
}