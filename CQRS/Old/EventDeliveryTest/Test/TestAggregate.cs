using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;

namespace EventDeliveryTest.Test
{
    public class TestAggregate : AggregateRoot, IEventExecutor<TestEvent>
    {
        public static readonly Guid IdField = new Guid("10FC8F67-4F4F-427B-A734-D6F2BD22A376");

        public string LastValue
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public async Task SetLastValue(string value) 
            => await ProvideEvent(new TestEvent(IdField, Version, value));

        Task IEventExecutor<TestEvent>.Apply(TestEvent eEvent)
        {
            LastValue = eEvent.Result;
            return Task.CompletedTask;
        }
    }
}