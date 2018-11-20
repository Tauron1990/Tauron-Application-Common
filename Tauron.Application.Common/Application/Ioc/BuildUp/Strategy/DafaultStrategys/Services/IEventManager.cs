using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public interface IEventManager
    {
        void AddEventHandler(string topic, Delegate handler, ErrorTracer errorTracer);

        void AddEventHandler(string topic, MethodInfo handler, object target, ErrorTracer errorTracer);

        void AddPublisher(string topic, EventInfo eventInfo, object publisher, ErrorTracer errorTracer);

    }
}