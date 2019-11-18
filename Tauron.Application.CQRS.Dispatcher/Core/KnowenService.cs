using System.Threading;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public class KnowenService
    {
        private string[] _subscriptions;

        public string Name { get; set; }

        public string[] Subscriptions
        {
            get => _subscriptions;
            set => _subscriptions = value;
        }

        public KnowenService()
        {
            
        }

        public KnowenService(string name, string[] subscriptions)
        {
            Name = name;
            Subscriptions = subscriptions;
        }

        public void SafeExcange(string[] events) => Interlocked.Exchange(ref events, events);
    }
}