using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tauron.Application.Common.Updater.Impl
{
    internal static class DebuggerService
    {
        internal static string Result { get; set; } = "";
        internal static bool Debug { get; set; }
        internal static bool SkipFirst { get; set; } = true;

        [Conditional("DEBUG")]
        internal static void StartDebug()
        {
            Debug = true;
            SkipFirst = false;
        }

        internal static string[] GetCommandLine()
        {
            if (Debug) return ParseLine(Result);

            return Environment.GetCommandLineArgs();
        }

        private static string[] ParseLine(string line)
        {
            List<string> args = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool block = false;

            foreach (var c in line)
            {
                if (!block && char.IsWhiteSpace(c) && builder.Length != 0)
                {
                    args.Add(builder.ToString());
                    builder.Clear();
                }

                else if (c == '"')
                    block = !block;

                else
                    builder.Append(c);
            }

            if(builder.Length != 0)
                args.Add(builder.ToString());

            return args.ToArray();
        }
    }
}