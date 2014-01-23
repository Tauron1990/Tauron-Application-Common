// The file SingleInstanceApp.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleInstanceApp.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   This class checks to make sure that only one instance of
//   this application is running at a time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using Tauron.Interop;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>
    ///     This class checks to make sure that only one instance of
    ///     this application is running at a time.
    /// </summary>
    /// <typeparam name="TApplication">
    /// </typeparam>
    /// <remarks>
    ///     Note: this class should be used with some caution, because it does no
    ///     security checking. For example, if one instance of an app that uses this class
    ///     is running as Administrator, any other instance, even if it is not
    ///     running as Administrator, can activate it with command line arguments.
    ///     For most apps, this will not be much of an issue.
    /// </remarks>
    [PublicAPI]
    public static class SingleInstance<TApplication>
        where TApplication : ISingleInstanceApp
    {
        #region Constants

        /// <summary>Suffix to the channel name.</summary>
        private const string ChannelNameSuffix = "SingeInstanceIPCChannel";

        /// <summary>String delimiter used in channel names.</summary>
        private const string Delimiter = ":";

        /// <summary>IPC protocol used (string).</summary>
        private const string IpcProtocol = "ipc://";

        /// <summary>Remote service name.</summary>
        private const string RemoteServiceName = "SingleInstanceApplicationService";

        #endregion

        #region Static Fields

        /// <summary>The _app.</summary>
        [CanBeNull] private static ISingleInstanceApp _app;

        /// <summary>IPC channel for communications.</summary>
        private static IpcServerChannel channel;

        /// <summary>List of command line arguments for the application.</summary>
        private static IList<string> commandLineArgs;

        /// <summary>Application mutex.</summary>
        private static Mutex singleInstanceMutex;

        #endregion

        #region Public Properties

        /// <summary>Gets list of command line arguments for the application.</summary>
        /// <value>The command line args.</value>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IList<string> CommandLineArgs
        {
            get { return commandLineArgs; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Cleans up single-instance code, clearing shared resources, mutexes, etc.</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Cleanup()
        {
            _app = null;
            if (singleInstanceMutex != null)
            {
                singleInstanceMutex.Close();
                singleInstanceMutex = null;
            }

            if (channel == null) return;

            ChannelServices.UnregisterChannel(channel);
            channel = null;
        }

        /// <summary>
        ///     Checks if the instance of the application attempting to start is the first instance.
        ///     If not, activates the first instance.
        /// </summary>
        /// <param name="uniqueName">
        ///     The unique Name.
        /// </param>
        /// <param name="application">
        ///     The application.
        /// </param>
        /// <returns>
        ///     True if this is the first instance of the application.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static bool InitializeAsFirstInstance([NotNull] string uniqueName, TApplication application)
        {
            Contract.Requires<ArgumentNullException>(uniqueName != null, "uniqueName");
            Contract.Requires<ArgumentNullException>(application != null, "application");

            commandLineArgs = GetCommandLineArgs(uniqueName);

            // Build unique application Id and the IPC channel name.
            string applicationIdentifier = uniqueName + Environment.UserName;

            string channelName = string.Concat(applicationIdentifier, Delimiter, ChannelNameSuffix);

            // Create mutex based on unique application Id to check if this is the first instance of the application.
            bool firstInstance;
            singleInstanceMutex = new Mutex(true, applicationIdentifier, out firstInstance);
            if (firstInstance)
            {
                CreateRemoteService(channelName);
                _app = application;
            }
            else SignalFirstInstance(channelName, commandLineArgs);

            return firstInstance;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Activates the first instance of the application with arguments from a second instance.
        /// </summary>
        /// <param name="args">
        ///     List of arguments to supply the first instance of the application.
        /// </param>
        private static void ActivateFirstInstance([CanBeNull] IList<string> args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");

            // Set main window state and process command line args
            if (_app == null) return;

            _app.SignalExternalCommandLineArgs(args);
        }

        /// <summary>
        ///     Callback for activating first instance of the application.
        /// </summary>
        /// <param name="arg">
        ///     Callback argument.
        /// </param>
        private static void ActivateFirstInstanceCallback([CanBeNull] object arg)
        {
            Contract.Requires<ArgumentNullException>((arg as IList<string>) != null, "arg");

            // Get command line args to be passed to first instance
            var args = arg as IList<string>;
            ActivateFirstInstance(args);
        }

        /// <summary>
        ///     Creates a remote service for communication.
        /// </summary>
        /// <param name="channelName">
        ///     Application's IPC channel name.
        /// </param>
        private static void CreateRemoteService([NotNull] string channelName)
        {
            var serverProvider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};
            IDictionary props = new Dictionary<string, string>();

            props["name"] = channelName;
            props["portName"] = channelName;
            props["exclusiveAddressUse"] = "false";

            // Create the IPC Server channel with the channel properties
            channel = new IpcServerChannel(props, serverProvider);

            // Register the channel with the channel services
            ChannelServices.RegisterChannel(channel, true);

            // Expose the remote service with the REMOTE_SERVICE_NAME
            var remoteService = new IPCRemoteService();
            RemotingServices.Marshal(remoteService, RemoteServiceName);
        }

        /// <summary>
        ///     Gets command line args - for ClickOnce deployed applications, command line args may not be passed directly, they
        ///     have to be retrieved.
        /// </summary>
        /// <param name="uniqueApplicationName">
        ///     The unique Application Name.
        /// </param>
        /// <returns>
        ///     List of command line arg strings.
        /// </returns>
        [NotNull]
        private static IList<string> GetCommandLineArgs([NotNull] string uniqueApplicationName)
        {
            Contract.Requires<ArgumentNullException>(uniqueApplicationName != null, "uniqueApplicationName");

            string[] args = null;
            if (AppDomain.CurrentDomain.ActivationContext == null) // The application was not clickonce deployed, get args from standard API's
                args = Environment.GetCommandLineArgs();
            else
            {
                // The application was clickonce deployed
                // Clickonce deployed apps cannot recieve traditional commandline arguments
                // As a workaround commandline arguments can be written to a shared location before
                // the app is launched and the app can obtain its commandline arguments from the
                // shared location
                string appFolderPath =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        uniqueApplicationName);

                string cmdLinePath = Path.Combine(appFolderPath, "cmdline.txt");
                if (File.Exists(cmdLinePath))
                {
                    try
                    {
                        using (TextReader reader = new StreamReader(cmdLinePath, Encoding.Unicode)) args = NativeMethods.CommandLineToArgvW(reader.ReadToEnd());

                        File.Delete(cmdLinePath);
                    }
                    catch (IOException)
                    {
                    }
                }
            }

            if (args == null) args = new string[] {};

            return new List<string>(args);
        }

        /// <summary>
        ///     Creates a client channel and obtains a reference to the remoting service exposed by the server -
        ///     in this case, the remoting service exposed by the first instance. Calls a function of the remoting service
        ///     class to pass on command line arguments from the second instance to the first and cause it to activate itself.
        /// </summary>
        /// <param name="channelName">
        ///     Application's IPC channel name.
        /// </param>
        /// <param name="args">
        ///     Command line arguments for the second instance, passed to the first instance to take appropriate action.
        /// </param>
        private static void SignalFirstInstance([NotNull] string channelName, [NotNull] IList<string> args)
        {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            string remotingServiceUrl = IpcProtocol + channelName + "/" + RemoteServiceName;

            // Obtain a reference to the remoting service exposed by the server i.e the first instance of the application
            var firstInstanceRemoteServiceReference =
                (IPCRemoteService) RemotingServices.Connect(typeof (IPCRemoteService), remotingServiceUrl);

            // Check that the remote service exists, in some cases the first instance may not yet have created one, in which case
            // the second instance should just exit
            if (firstInstanceRemoteServiceReference != null) // Invoke a method of the remote service exposed by the first instance passing on the command line
                // arguments and causing the first instance to activate itself
                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
        }

        #endregion

        /// <summary>
        ///     Remoting service class which is exposed by the server i.e the first instance and called by the second instance
        ///     to pass on the command line arguments to the first instance and cause it to activate itself.
        /// </summary>
        [DebuggerNonUserCode]
        private class IPCRemoteService : MarshalByRefObject
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Remoting Object's ease expires after every 5 minutes by default. We need to override the InitializeLifetimeService
            ///     class
            ///     to ensure that lease never expires.
            /// </summary>
            /// <returns>Always null.</returns>
            public override object InitializeLifetimeService()
            {
                return null;
            }

            /// <summary>
            ///     Activates the first instance of the application.
            /// </summary>
            /// <param name="args">
            ///     List of arguments to pass to the first instance.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public void InvokeFirstInstance([NotNull] IList<string> args)
            {
                // Do an asynchronous call to ActivateFirstInstance function
                var thread = new Thread(ActivateFirstInstanceCallback);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(args);
            }

            #endregion
        }
    }
}