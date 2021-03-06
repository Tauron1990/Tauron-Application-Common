﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ServiceManager.Core.Installation.Core;

namespace ServiceManager.Core.Installation.Tasks
{
    public class ExtractToTempTask : InstallerTask
    {
        private readonly string _tempPath = Path.Combine(AppContext.BaseDirectory, "Temp");

        public override string Title => "Extrahieren";

        public override Task Prepare(InstallerContext context)
        {
            Content = "Update wird Extrahiert";

            return Task.CompletedTask;
        }

        public override Task<string?> RunInstall(InstallerContext context)
        {
            if (!context.MetaData.TryGetTypedValue(MetaKeys.ArchiveFile, out ZipArchive? zipArchive))
#pragma warning disable CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.
                return Task.FromResult("Zip Erchive nicht gefunden"!);
#pragma warning restore CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.

            DeleteTemp();

            zipArchive.ExtractToDirectory(_tempPath);
            context.MetaData[MetaKeys.TempLocation] = _tempPath;

#pragma warning disable CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.
            return Task.FromResult<string>(null!);
#pragma warning restore CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.
        }

        public override void Dispose() 
            => DeleteTemp();

        private void DeleteTemp()
        {
            if (Directory.Exists(_tempPath))
                Directory.Delete(_tempPath, true);
        }
    }
}