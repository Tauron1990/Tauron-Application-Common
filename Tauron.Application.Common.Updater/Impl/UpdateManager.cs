using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CommandLine;
using Tauron.Application.Common.Updater.Provider;
using Tauron.Application.Common.Updater.Service;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class UpdateManager : IUpdateManager
    {
        private class Options
        {
            [Option(StepCommand)]
            public string Step { get; set; }

            [Option(KillProcessCommand)]
            public string KillProcess { get; set; }

            [Option(TargetCommand)]
            public string Target { get; set; }

            [Option(BasePathCommand)]
            public string BasePath { get; set; }

            [Option(VersionCommand)]
            public string Version { get; set; }
        }

        private const string StepCommand = "Step";
        private const string Step2Parm = "Setup";
        private const string Step3Parm = "Completion";
        private const string KillProcessCommand = "KillProcess";
        private const string TargetCommand = "Target";
        private const string BasePathCommand = "BasePath";
        private const string VersionCommand = "NewVersion";

        private class PrivateUpdate : IUpdate
        {
            public PrivateUpdate(Release release)
            {
                Release = release;
            }

            public Release Release { get; }
        }

        public InstallerStade Setup()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                var parsed = Parser.Default.ParseArguments<Options>(args) as Parsed<Options>;
                var commands = parsed?.Value ?? new Options();

                if (string.IsNullOrWhiteSpace(commands.Step)) return InstallerStade.NoUpdate;

                switch (commands.Step)
                {
                    case Step2Parm:
                        KillProcess(commands.KillProcess);
                        string basePath = commands.BasePath.Trim('"');
                        string target = commands.Target.Trim('"');
                        Version newVersion = Version.Parse(commands.Version);

                        ExecuteSetup(basePath, target, newVersion);
                        return InstallerStade.Shudown;
                    case Step3Parm:
                        KillProcess(commands.KillProcess);
                        UpdaterService.Configuration.Provider.UpdaterFilesLocation.DeleteDirectory(true);
                        UpdaterService.Configuration.StartCleanUp?.Invoke();
                        return InstallerStade.Compled;
                    default:
                        return InstallerStade.NoUpdate;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public IUpdate CheckUpdate()
        {
            Version currentVersion = UpdaterService.Configuration.CurrentVersion;
            var currrent = GetCurrent();
            Version newesVersion = currrent.Version;

            if (newesVersion > currentVersion)
                return new PrivateUpdate(currrent);
            return null;
        }

        public void InstallUpdate(IUpdate update)
        {
            var configuration = UpdaterService.Configuration;
            var downloaded = UpdaterService.InstallManager.DownloadUpdate(update);

            string files = configuration.Provider.Preperator.Prepare(downloaded);

            string targetFile = files.CombinePath(configuration.SetupFile);
            var processId = Process.GetCurrentProcess().Id;

            string commandLine = $"--{KillProcessCommand} {processId} --{StepCommand} {Step2Parm}  --{VersionCommand} {configuration.CurrentVersion} --{TargetCommand} \"{downloaded}\" --{BasePathCommand} \"{AppDomain.CurrentDomain.BaseDirectory}\"";

            if (DebuggerService.Debug)
                DebuggerService.Result = commandLine;
            else
                Process.Start(targetFile, commandLine);
        }

        public IUpdate GetLasted()
        {
            return new PrivateUpdate(GetCurrent());
        }

        private Release GetCurrent()
        {
            var temp = UpdaterService.Configuration.Provider.GetReleases().ToArray();
            return temp.OrderByDescending(t => t.Version).First();
        }

        private void KillProcess(string id)
        {
            if (DebuggerService.Debug)
                return;

            try
            {
                var process = Process.GetProcessById(Int32.Parse(id));

                if (!process.WaitForExit(TimeSpan.FromSeconds(30).Milliseconds))
                    process.Kill();
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidOperationException || e is Win32Exception)
            {
            }

        }

        //private string GetCommandValue(HashSet<CommandLineProcessor.Command> commands, string name)
        //{
        //    return commands.First(c => c.Name == name).Parms[0];
        //}

        private void ExecuteSetup(string basePath, string filePath, Version oldVersion)
        {
            UpdaterService.InstallManager.ExecuteSetup(basePath, filePath, oldVersion);

            string targetFile = basePath.CombinePath(UpdaterService.Configuration.StartFile);
            var processId = Process.GetCurrentProcess().Id;

            string commandLine = $"--{KillProcessCommand} {processId} --{StepCommand} {Step3Parm}";

            if (DebuggerService.Debug)
                DebuggerService.Result = commandLine;
            else
                Process.Start(targetFile, commandLine);
        }
    }
}