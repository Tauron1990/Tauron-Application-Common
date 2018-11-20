using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.Windows.Impl
{
    [PublicAPI]
    [Export(typeof(ITauronEnviroment))]
    public class TauronEnviroment : ITauronEnviroment
    {
        public static string AppRepository = "Tauron";

        private string _defaultPath;

        public string DefaultProfilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultPath))
                    _defaultPath = LocalApplicationData;

                _defaultPath.CreateDirectoryIfNotExis();

                return _defaultPath;
            }

            set => _defaultPath = value;
        }

        public string LocalApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath(AppRepository);

        public string LocalApplicationTempFolder => LocalApplicationData.CombinePath("Temp");

        public string LocalDownloadFolder => SearchForFolder(KnownFolder.Downloads);

        public IEnumerable<string> GetProfiles(string application)
        {
            return
                DefaultProfilePath.CombinePath(application)
                    .EnumerateDirectorys()
                    .Select(ent => ent.Split('\\').Last());
        }

        public string SearchForFolder(Guid id)
        {
            if (NativeMethods.SHGetKnownFolderPath(id, 0, IntPtr.Zero, out var pPath) != 0) return string.Empty;

            var s = Marshal.PtrToStringUni(pPath);
            Marshal.FreeCoTaskMem(pPath);
            return Argument.CheckResult(s, "The Result Path was Null");

        }
    }
}