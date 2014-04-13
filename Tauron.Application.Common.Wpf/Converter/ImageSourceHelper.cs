#region

using System;
using System.Diagnostics;
using System.Windows.Markup;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    internal static class ImageSourceHelper
    {
        #region Public Methods and Operators

        public static bool Enter([CanBeNull] string imageSource, [NotNull] IServiceProvider provider)
        {
            if (imageSource != null) return false;

            if (Logger.IsLoggingEnabled())
            {
                Logger.Write(
                    new LogEntry
                    {
                        Severity = TraceEventType.Warning,
                        Message =
                            string.Format(
                                "InmageSource are null. {0}",
                                provider.GetService<IProvideValueTarget>().TargetObject)
                    });
            }

            return true;
        }

        public static bool Exit([NotNull] string imageSource, bool isNull)
        {
            if (!isNull) return false;

            if (Logger.IsLoggingEnabled())
            {
                Logger.Write(
                    new LogEntry
                    {
                        Severity = TraceEventType.Warning,
                        Message = string.Format("Inmage not Found: {0}.", imageSource)
                    });
            }

            return true;
        }

        [NotNull]
        public static string ResolveAssembly([NotNull] string assembly, [NotNull] IServiceProvider provider)
        {
            if (assembly != "this") return assembly;

            var target = provider.GetService<IProvideValueTarget>();
            return target == null ? assembly : target.TargetObject.GetType().Assembly.FullName;
        }

        #endregion
    }
}