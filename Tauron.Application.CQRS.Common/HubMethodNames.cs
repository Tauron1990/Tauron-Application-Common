namespace Tauron.Application.CQRS.Common
{
    public static class HubMethodNames
    {
        //public static class DispatcherCommand
        //{
        //    public const string StopDispatcher = nameof(StopDispatcher);

        //    public const string StartDispatcher = nameof(StartDispatcher);
        //}

        public static class HeartbeatNames
        {
            public const string Heartbeat = nameof(Heartbeat);

            public const string StillConnected = nameof(StillConnected);
        }

        public const string Subscribe = nameof(Subscribe);

        public const string PropagateEvent = nameof(PropagateEvent);

        public const string SendingSuccseded = nameof(SendingSuccseded);

        public const string PublishEvent = nameof(PublishEvent);

        public const string PublishEventGroup = nameof(PublishEventGroup);

        public const string PublishEventToClient = nameof(PublishEventToClient);
    }
}