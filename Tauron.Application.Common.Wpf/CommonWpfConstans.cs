using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public static class CommonWpfConstans
    {
        public const string CommonCategory = "Tauron.Application.Common.Wpf";
        public const string CommonExceptionPolicy = "Tauron.Application.Common.Wpf.Policy";

        [StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] params object[] parms)
        {
            Contract.Requires<ArgumentNullException>(format != null, "format");
            Contract.Requires<ArgumentNullException>(parms != null, "parms");

            string realMessage = parms.Length == 0 ? format : string.Format(format, parms);
            Logger.Write(realMessage, CommonCategory, -1, -1, isError ? TraceEventType.Error : TraceEventType.Warning);
        }
    }
}
