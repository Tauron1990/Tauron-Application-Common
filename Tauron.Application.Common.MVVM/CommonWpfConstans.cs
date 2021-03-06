﻿using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    public static class CommonWpfConstans
    {
        public const string CommonCategory = "Tauron.Application.Common.Wpf";

        [StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] [ItemNotNull] params object[] parms)
        {
            Argument.NotNull(format, nameof(format));
            Argument.NotNull(parms, nameof(parms));

            var realMessage = parms.Length == 0 ? format : string.Format(format, parms);
            LogManager.GetLogger(CommonCategory).Log(isError ? LogLevel.Error : LogLevel.Warn, realMessage, parms);
        }
    }
}