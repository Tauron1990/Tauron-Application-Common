using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method)]
    [PublicAPI]
    public sealed class InjectEventAttribute : Attribute
    {
        public InjectEventAttribute([NotNull] string topic)
        {
            Argument.NotNull(topic, nameof(topic));

            Topic = topic;
        }

        public string Topic { get; }
    }
}