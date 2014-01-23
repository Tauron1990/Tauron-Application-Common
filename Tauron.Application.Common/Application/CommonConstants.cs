using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public static class CommonConstants
    {
        public const string CommonCategory = "Tauron.Application.Common";
        public const string CommonExceptionPolicy = "Tauron.Application.Common.Policy";

        [PublicAPI, StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] params object[] parmsObjects)
        {
            Contract.Requires<ArgumentNullException>(format != null, "format");
            Contract.Requires<ArgumentNullException>(parmsObjects != null, "parmsObjects");

            string realMessage = parmsObjects.Length == 0 ? format : string.Format(format, parmsObjects);
            Logger.Write(realMessage, CommonCategory, -1,-1, isError ? TraceEventType.Error : TraceEventType.Warning);
        }
    }
}
