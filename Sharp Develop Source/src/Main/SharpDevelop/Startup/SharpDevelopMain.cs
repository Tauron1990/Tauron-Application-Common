// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.SharpDevelop.Logging;
using ICSharpCode.SharpDevelop.Sda;

namespace ICSharpCode.SharpDevelop.Startup
{
    /// <summary>
    /// This Class is the Core main class, it starts the program.
    /// </summary>
    internal static class SharpDevelopMain
    {
        private static string[] _commandLineArgs;

        public static string[] CommandLineArgs
        {
            get
            {
                return _commandLineArgs;
            }
        }

        private static bool UseExceptionBox
        {
            get
            {
#if DEBUG
                return !Debugger.IsAttached && _commandLineArgs.All(arg => !arg.Contains("noExceptionBox"));
#endif
            }
        }

        /// <summary>
        /// Starts the core of SharpDevelop.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            _commandLineArgs = args; // Needed by UseExceptionBox

            // Do not use LoggingService here (see comment in Run(string[]))
            if (UseExceptionBox)
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    try
                    {
                        HandleMainException(ex);
                    }
                    catch (Exception loadError)
                    {
                        // HandleMainException can throw error when log4net is not found
                        MessageBox.Show(loadError.ToString(), "Critical error (Logging service defect?)");
                    }
                }
            }
            else
            {
                Run();
            }
        }

        private static void HandleMainException(Exception ex)
        {
            LoggingService.Fatal(ex);
            try
            {
                Application.Run(new ExceptionBox(ex, "Unhandled exception terminated SharpDevelop", true));
            }
            catch
            {
                MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
            }
        }

        private static void Run()
        {
            // DO NOT USE LoggingService HERE!
            // LoggingService requires ICSharpCode.Core.dll and log4net.dll
            // When a method containing a call to LoggingService is JITted, the
            // libraries are loaded.
            // We want to show the SplashScreen while those libraries are loading, so
            // don't call LoggingService.

#if DEBUG
            Control.CheckForIllegalCrossThreadCalls = true;
#endif
            bool noLogo = false;

            Application.SetCompatibleTextRenderingDefault(false);
            SplashScreenForm.SetCommandLineArgs(_commandLineArgs);

            if(SplashScreenForm.GetParameterList().Any(parameter => "nologo".Equals(parameter, StringComparison.OrdinalIgnoreCase)))
            {
                noLogo = true;
            }

            if (!CheckEnvironment()) return;

            if (!noLogo)
            {
                SplashScreenForm.ShowSplashScreen();
            }
            try
            {
                RunApplication();
            }
            finally
            {
                if (SplashScreenForm.SplashScreen != null)
                {
                    SplashScreenForm.SplashScreen.Dispose();
                }
            }
        }

        private static bool CheckEnvironment()
        {
            // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
            // used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
            if (Environment.Version < new Version(4, 0, 30319, 17626))
            {
                MessageBox.Show(
                    "This version of SharpDevelop requires .NET 4.5 RC. You are using: " + Environment.Version,
                    "SharpDevelop");
                return false;
            }
            // Work around a WPF issue when %WINDIR% is set to an incorrect path
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows,
                                                      Environment.SpecialFolderOption.DoNotVerify);
            if (Environment.GetEnvironmentVariable("WINDIR") != windir)
            {
                Environment.SetEnvironmentVariable("WINDIR", windir);
            }
            return true;
        }

        private static void RunApplication()
        {
            // The output encoding differs based on whether SharpDevelop is a console app (debug mode)
            // or Windows app (release mode). Because this flag also affects the default encoding
            // when reading from other processes' standard output, we explicitly set the encoding to get
            // consistent behaviour in debug and release builds of SharpDevelop.

#if DEBUG
            // Console apps use the system's OEM codepage, windows apps the ANSI codepage.
            // We'll always use the Windows (ANSI) codepage.
            try
            {
                Console.OutputEncoding = System.Text.Encoding.Default;
            }
            catch (IOException)
            {
                // can happen if SharpDevelop doesn't have a console
            }
#endif

            LoggingService.Info("Starting SharpDevelop...");
            try
            {
                var startup = new StartupSettings {UseSharpDevelopErrorHandler = UseExceptionBox};
#if DEBUG
#endif

                Assembly exe = typeof (SharpDevelopMain).Assembly;
                startup.ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
                startup.AllowUserAddIns = true;

                string configDirectory = ConfigurationManager.AppSettings["settingsPath"];
                if (String.IsNullOrEmpty(configDirectory))
                {
                    startup.ConfigDirectory =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                     "ICSharpCode/SharpDevelop" + RevisionClass.Major);
                }
                else
                {
                    startup.ConfigDirectory = Path.Combine(Path.GetDirectoryName(exe.Location), configDirectory);
                }

                startup.DomPersistencePath = ConfigurationManager.AppSettings["domPersistencePath"];
                if (string.IsNullOrEmpty(startup.DomPersistencePath))
                {
                    startup.DomPersistencePath = Path.Combine(Path.GetTempPath(),
                                                              "SharpDevelop" + RevisionClass.Major + "." +
                                                              RevisionClass.Minor);
#if DEBUG
                    startup.DomPersistencePath = Path.Combine(startup.DomPersistencePath, "Debug");
#endif
                }
                else if (startup.DomPersistencePath == "none")
                {
                    startup.DomPersistencePath = null;
                }

                startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));

                // allows testing addins without having to install them
                foreach (string parameter in SplashScreenForm.GetParameterList().Where(parameter => parameter.StartsWith("addindir:", StringComparison.OrdinalIgnoreCase)))
                {
                    startup.AddAddInsFromDirectory(parameter.Substring(9));
                }

                var host = new SharpDevelopHost(AppDomain.CurrentDomain, startup);

                string[] fileList = SplashScreenForm.GetRequestedFileList();
                if (fileList.Length > 0)
                {
                    if (!LoadFilesInPreviousInstance(fileList)) return;
                    LoggingService.Info("Aborting startup, arguments will be handled by previous instance");
                    return;
                }

                host.BeforeRunWorkbench += delegate
                {
                    if (SplashScreenForm.SplashScreen == null) return;
                    SplashScreenForm.SplashScreen.BeginInvoke(
                        new MethodInvoker(SplashScreenForm.SplashScreen.Dispose));
                    SplashScreenForm.SplashScreen = null;
                };

                var workbenchSettings = new WorkbenchSettings {RunOnNewThread = false};
                foreach (string file in fileList)
                {
                    workbenchSettings.InitialFileList.Add(file);
                }
                SDTraceListener.Install();
                host.RunWorkbench(workbenchSettings);
            }
            finally
            {
                LoggingService.Info("Leaving RunApplication()");
            }
        }

        private static bool LoadFilesInPreviousInstance(string[] fileList)
        {
            try
            {
                if (fileList.Any(file => SD.ProjectService.IsSolutionOrProjectFile(FileName.Create(file))))
                {
                    return false;
                }
                return SingleInstanceHelper.OpenFilesInPreviousInstance(fileList);
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return false;
            }
        }
    }
}
