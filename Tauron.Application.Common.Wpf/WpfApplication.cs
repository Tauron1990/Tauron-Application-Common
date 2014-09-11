// The file WpfApplication.cs is part of Tauron.Application.Common.Wpf.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Tauron.Application.Composition;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The wpf application.</summary>
    [PublicAPI]
    public class WpfApplication : CommonApplication
    {
        protected class InternalLoggerDefaultFormatter : IFormatterBuilder
        {
            private readonly string _pattern;

            public InternalLoggerDefaultFormatter([NotNull] string pattern)
            {
                Contract.Requires<ArgumentNullException>(pattern != null, "pattern");

                _pattern = pattern;
            }

            [NotNull]
            public FormatterData GetFormatterData()
            {
                return new TextFormatterData(_pattern);
            }
        }

        private string _defaultFormatterPattern = "{message}{newline}";

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfApplication" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WpfApplication" /> Klasse.
        /// </summary>
        /// <param name="doStartup">
        ///     The do startup.
        /// </param>
        public WpfApplication(bool doStartup)
            : base(doStartup, new SplashService(), new WpfIuiControllerFactory())
        {
            CatalogList = "Catalogs.xaml";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the container.</summary>
        public override IContainer Container
        {
            get { return CompositionServices.Container; }

            set { CompositionServices.Container = value; }
        }

        /// <summary>Gets or sets the theme dictionary.</summary>
        [CanBeNull]
        public string ThemeDictionary { get; set; }

        [NotNull]
        public static System.Windows.Application CurrentWpfApplication
        {
            get { return System.Windows.Application.Current; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The run.</summary>
        public static void Run<TApp>() where TApp : WpfApplication, new()
        {
            WpfApplicationController.Initialize();
            WpfApplication app = new TApp();
            UiSynchronize.Synchronize.Invoke(app.ConfigSplash);
            app.OnStartup(Environment.GetCommandLineArgs());
        }

        #endregion

        #region Methods

        /// <summary>The config splash.</summary>
        protected virtual void ConfigSplash()
        {
        }

        /// <summary>The load resources.</summary>
        protected override void LoadResources()
        {
            if (string.IsNullOrEmpty(ThemeDictionary)) return;

            QueueWorkitem(
                () =>
                WpfApplicationController.Application.Resources.MergedDictionaries.Add(
                    System.Windows.Application
                          .LoadComponent(
                              new Uri
                                  (
                                  string
                                      .Format
                                      (@"/{0};component/{1}",
                                       SourceAssembly,
                                       ThemeDictionary),
                                  UriKind
                                      .Relative))
                          .CastObj
                        <ResourceDictionary>
                        ()),
                true);
        }

        /// <summary>
        ///     The main window closed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void MainWindowClosed(object sender, EventArgs e)
        {
            Shutdown();
            System.Windows.Application.Current.Shutdown();
            Container.Dispose();
        }

        [NotNull]
        public string DefaultFormatterPattern
        {
            get { return _defaultFormatterPattern; }
            set
            {
                if(string.IsNullOrWhiteSpace(value)) return;
                _defaultFormatterPattern = value;
            }
        }

        protected override IConfigurationSource CreateConfiguration()
        {
            var builder = new ConfigurationSourceBuilder();
            var source = new DictionaryConfigurationSource();

            var exConfig = builder.ConfigureExceptionHandling();
            exConfig.GivenPolicyWithName(CommonConstants.CommonExceptionPolicy)
                    .ForExceptionType<Exception>()
                    .LogToCategory(CommonConstants.CommonCategory)
                    .ThenNotifyRethrow();
            exConfig.GivenPolicyWithName(CommonWpfConstans.CommonExceptionPolicy)
                .ForExceptionType<Exception>()
                .LogToCategory(CommonWpfConstans.CommonCategory)
                .ThenNotifyRethrow();
            ConfigureExceptionHandling(exConfig);

            var logConfig = builder.ConfigureLogging();

            "Logs".ClearDirectory();

            CrateDefaultRollingFile(logConfig.LogToCategoryNamed(CommonConstants.CommonCategory).SendTo
                                             .RollingFile("Tauron.Application.Common.File"), "Logs\\Tauron.Application.Common.log");
            CrateDefaultRollingFile(logConfig.LogToCategoryNamed(CommonWpfConstans.CommonCategory).SendTo
                                 .RollingFile("Tauron.Application.Common.Wpf.File"), "Logs\\Tauron.Application.Common.Wpf.log");
            
            ConfigureLogging(logConfig);

            builder.UpdateConfigurationWithReplace(source);

            return source;
        }

        protected void CrateDefaultRollingFile([NotNull] ILoggingConfigurationSendToRollingFileTraceListener listener, [NotNull] string path)
        {
            Contract.Requires<ArgumentNullException>(listener != null, "listener");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            path.GetFullPath().CreateDirectoryIfNotExis();

            listener.ToFile(path)
                    .WhenRollFileExists(RollFileExistsBehavior.Increment)
                    .CleanUpArchivedFilesWhenMoreThan(4)
                    .FormatWith(new InternalLoggerDefaultFormatter(DefaultFormatterPattern));
        }

        protected virtual void ConfigureExceptionHandling(
            [NotNull] IExceptionConfigurationGivenPolicyWithName exceptionConfiguration)
        {
            Contract.Requires<ArgumentNullException>(exceptionConfiguration != null, "exceptionConfiguration");
        }

        protected virtual void ConfigureLogging([NotNull] ILoggingConfigurationStart loggingConfiguration)
        {
            Contract.Requires<ArgumentNullException>(loggingConfiguration != null, "loggingConfiguration");
        }

        protected override IContainer CreateContainer()
        {
            var con = base.CreateContainer();

            con.Register(new PropertyModelExtension());

            return con;
        }

        protected override void OnStartupError(Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        #endregion
    }
}