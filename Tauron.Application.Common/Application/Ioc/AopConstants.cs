using System.Reflection;

namespace Tauron.Application.Ioc
{
    public static class AopConstants
    {
        public const string ContextMetadataName = "ContextName";

        public const BindingFlags DefaultBindingFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        public const string DefaultExportFactoryName = "Internal";

        public const string EventAdder = "add_";

        public const string EventMetodMetadataName = "EventMethod";

        public const string EventRemover = "remove_";

        public const string EventTopicMetadataName = "EventTopic";

        public const string InterceptMetadataName = "Intercept";

        public const string InternalUniversalInterceptorName = "Internal@Universal";

        public const string LiftimeMetadataName = "LiftimeMetadata";

        public const string ParameterMetadataName = "Parameters";

        public const string PropertyGetter = "get_";

        public const string PropertySetter = "set_";
    }
}