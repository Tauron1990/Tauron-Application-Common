using System;
using Tauron.Application.CQRS.Client.Events;

namespace EventDeliveryTest.Test
{
    public class TestEvent : EventBase
    {
        public string Result { get; set; }

        public TestEvent(Guid id, long version, string result) : base(id, version)
        {
            Result = result;
        }

        public TestEvent()
        {
            
        }
    }
}