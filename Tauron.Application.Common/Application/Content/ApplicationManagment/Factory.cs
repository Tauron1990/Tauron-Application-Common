using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [PublicAPI]
    public static class Factory
    {
        public static void Update(object toBuild)
        {
            Argument.NotNull(toBuild, nameof(toBuild));

            var errorTracer = new ErrorTracer();

            CommonApplication.Current.Container.BuildUp(toBuild, errorTracer);
            if (errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);
        }
        
        [NotNull]
        public static TObject Object<TObject>([NotNull] params object[] args)
            where TObject : class
        {
            Argument.NotNull(args, nameof(args));

            var tracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(typeof(TObject), tracer, new BuildParameter[0], args);
            if (tracer.Exceptional)
                throw new BuildUpException(tracer);

            return (TObject) val;
        }

        [NotNull]
        public static object Object([NotNull] Type targetType, [NotNull] params object[] args)
        {
            Argument.NotNull(targetType, nameof(targetType));
            Argument.NotNull(args, nameof(args));

            var errorTracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(targetType, errorTracer, new BuildParameter[0], args);
            if (errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);

            return val;
        }
    }
}