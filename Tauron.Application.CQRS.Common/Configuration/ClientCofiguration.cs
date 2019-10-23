using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Tauron.Application.CQRS.Common.Configuration
{
    [PublicAPI]
    public class ClientCofiguration
    {
        private ImmutableDictionary<string, HashSet<Type>> _handlerRegistry = ImmutableDictionary<string, HashSet<Type>>.Empty;
        private string _serviceName = string.Empty;

        public string EventHubUrl { get; set; } = string.Empty;

        public string EventServerApiUrl { get; set; } = string.Empty;

        public string PersistenceApiUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = string.Empty;

        public string ServiceName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_serviceName))
                    throw new InvalidOperationException("Need Servicename for Operation");
                return _serviceName;
            }
            set => _serviceName = value;
        }

        public IDictionary<string, HashSet<Type>> GetHandlers () => _handlerRegistry;
        
        public bool Memory { get; set; }

        private void AddHandler(string name, Type type)
        {
            if (_handlerRegistry.TryGetValue(name, out var list)) list.Add(type);
            else _handlerRegistry = _handlerRegistry.Add(name, new HashSet<Type> {type});
        }

        public bool IsHandlerRegistrated<TCommand, THandler>() =>
            _handlerRegistry.TryGetValue(typeof(TCommand).Name, out var list) && list.Contains(typeof(THandler));


        public ClientCofiguration SetUrls(Uri baseUrl, string serviceName, string apiKey)
        {
            ServiceName = serviceName;
            ApiKey = apiKey;

            BaseUrl = baseUrl.ToString();

            EventHubUrl = new Uri(baseUrl, "EventBus").ToString();
            EventServerApiUrl = new Uri(baseUrl, "Api/EventStore").ToString();
            PersistenceApiUrl = new Uri(baseUrl, "Api/Persistable").ToString();

            return this;
        }

        public void RegisterHandler(string name, Type type)
            => AddHandler(name, type);
    }
}