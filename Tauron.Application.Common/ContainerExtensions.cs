using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron
{
    [PublicAPI]
    public static class ContainerExtensions
    {
        private const string ErrorMessage = "Error on Return of Container Resolve";

        [NotNull]
        public static TType Resolve<TType>([NotNull] this IContainer con, params BuildParameter[] parameters) where TType : class
        {
            Argument.NotNull(con, nameof(con));
            return (TType)con.Resolve(typeof(TType), null, parameters);
        }

        public static TType Resolve<TType>([NotNull] this IContainer con, string name, bool optional, params BuildParameter[] buildParameters) where TType : class
        {
            Argument.NotNull(con, nameof(con));
            return con.Resolve(typeof(TType), name, optional, buildParameters) as TType;
        }

        public static object Resolve([NotNull] this IContainer con, [NotNull] Type @interface, string name, params BuildParameter[] buildParameters)
        {
            Argument.NotNull(con, nameof(con));
            Argument.NotNull(@interface, nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var expo = con.FindExport(@interface, name, tracer);
                return tracer.Exceptional ? null : con.BuildUp(expo, tracer, buildParameters);
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        public static object Resolve([NotNull] this IContainer con, [NotNull] Type @interface, string name, bool optional, BuildParameter[] buildParameters)
        {
            Argument.NotNull(con, nameof(con));
            Argument.NotNull(@interface, nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var data = con.FindExport(@interface, name, tracer, optional);

                if (tracer.Exceptional) return null;
                if (data != null) return con.BuildUp(data, tracer, buildParameters);

                if (optional) return null;
                return null;
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        public static IEnumerable<object> ResolveAll([NotNull] this IContainer con, [NotNull] Type @interface, string name, params BuildParameter[] buildParameters)
        {
            Argument.NotNull(con, nameof(con));
            Argument.NotNull(@interface, nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var temp = con.FindExports(@interface, name, tracer);
                if (tracer.Exceptional) yield break;

                foreach (var tempBuild in temp.Select(exportMetadata => con.BuildUp(exportMetadata, tracer, buildParameters)).TakeWhile(tempBuild => !tracer.Exceptional))
                    yield return tempBuild;
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }
        
        public static IEnumerable<TType> ResolveAll<TType>([NotNull] this IContainer con, string name)
        {
            Argument.NotNull(con, nameof(con));
            return ResolveAll(con, typeof(TType), name).Cast<TType>();
        }
    }
}