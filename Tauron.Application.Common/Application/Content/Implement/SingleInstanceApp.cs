using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
// ReSharper disable StaticMemberInGenericType

namespace Tauron.Application.Implement
{
    [PublicAPI]
    public static class SingleInstance<TApplication>
        where TApplication : ISingleInstanceApp
    {
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IList<string> CommandLineArgs => _commandLineArgs ?? throw new InvalidOperationException();

        public static void InitializeAsFirstInstance(Mutex mutex, string channelName, TApplication app)
        {
            _app = Argument.NotNull(app, nameof(app));
            _singleInstanceMutex = Argument.NotNull(mutex, nameof(mutex));
            CreateRemoteService(GetName(Argument.NotNull(channelName, nameof(channelName))));
        }

        public const string ChannelNameSuffix = "SingeInstanceIPCChannel";
        
        private const string RemoteServiceName = "SingleInstanceApplicationService";

        [CanBeNull]
        private static ISingleInstanceApp _app;

        [CanBeNull]
        private static NamedPipeServerStream _channel;

        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

        private static CancellationTokenSource _cancellation;

        private static Task _signaler; 

        [CanBeNull]
        private static IList<string> _commandLineArgs;
        
        [CanBeNull]
        private static Mutex _singleInstanceMutex;

        private static string GetName(string name) => RemoteServiceName + "-" + name + "-" + RemoteServiceName;

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Cleanup() => _cancellation.Cancel();

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static bool InitializeAsFirstInstance([NotNull] string uniqueName, TApplication application, IList<string> args)
        {
            _commandLineArgs = args;

            // Build unique application Id and the IPC channel name.
            var applicationIdentifier = uniqueName + Environment.UserName;

            var channelName = GetName(applicationIdentifier);

            // CreateResult mutex based on unique application Id to check if this is the first instance of the application.
            _singleInstanceMutex = new Mutex(true, applicationIdentifier, out var firstInstance);
            if (firstInstance)
            {
                CreateRemoteService(channelName);
                _app = application;
            }
            else
                SignalFirstInstance(channelName, _commandLineArgs);

            return firstInstance;
        }
        
        private static void ActivateFirstInstance([NotNull] IList<string> args) => _app?.SignalExternalCommandLineArgs(args);
        
        public static void CreateRemoteService([NotNull] string channelName)
        {
           _cancellation = new CancellationTokenSource();
            _signaler = Task.Factory.StartNew(() =>
            {
                using (_channel = new NamedPipeServerStream(channelName, PipeDirection.In, 1))
                {
                    try
                    {
                        while (!_cancellation.IsCancellationRequested)
                        {
                            _channel.WaitForConnectionAsync(_cancellation.Token).ContinueWith(t =>
                            {
                                using (var reader = new BinaryReader(_channel, Encoding.UTF8, true))
                                {
                                    try
                                    {
                                        int lenght = reader.ReadInt32();
                                        using (var array = new MemoryStream(reader.ReadBytes(lenght)))
                                            ActivateFirstInstance((IList<string>) _binaryFormatter.Deserialize(array));
                                    }
                                    catch (Exception e) when(e is Win32Exception || e is IOException || e is SerializationException) { }
                                }

                                Thread.Sleep(1000);
                            });
                        }
                    }
                    catch (OperationCanceledException) { }
                }
            });
        }
      
        public static void SignalFirstInstance([NotNull] string channelName, [NotNull] IList<string> args)
        {
            channelName = GetName(Argument.NotNull(channelName, nameof(channelName)));

            using (var clientStream = new NamedPipeClientStream(channelName))
            {
                clientStream.Connect(10000);
                if (!clientStream.IsConnected)
                    return;

                using (var writer = new BinaryWriter(clientStream))
                {
                    using (var mem = new MemoryStream())
                    {
                        _binaryFormatter.Serialize(mem, Argument.NotNull(args, nameof(args)));
                        mem.Position = 0;
                        var arr = mem.ToArray();
                        writer.Write(arr.Length);
                        writer.Write(arr);
                    }
                }
            }
        }
    }
}