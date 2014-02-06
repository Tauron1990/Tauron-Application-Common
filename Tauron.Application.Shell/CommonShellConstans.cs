using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell
{
    [PublicAPI]
    public static class CommonShellConstans
    {
        public const string CommonCategory = "Tauron.Application.Shell";
        public const string CommonExceptionPolicy = "Tauron.Application.Shell";

        [StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] params object[] parms)
        {
            Contract.Requires<ArgumentNullException>(format != null, "format");
            Contract.Requires<ArgumentNullException>(parms != null, "parms");

            string realMessage = parms.Length == 0 ? format : string.Format(format, parms);
            Logger.Write(realMessage, CommonCategory, -1, -1, isError ? TraceEventType.Error : TraceEventType.Warning);
        }

        public static void LogCommonInfo([NotNull] string format, [NotNull] params object[] parms)
        {
            Contract.Requires<ArgumentNullException>(format != null, "format");
            Contract.Requires<ArgumentNullException>(parms != null, "parms");

            string realMessage = parms.Length == 0 ? format : string.Format(format, parms);
            Logger.Write(realMessage, CommonCategory, -1, -1, TraceEventType.Information);
        }
    }
}
