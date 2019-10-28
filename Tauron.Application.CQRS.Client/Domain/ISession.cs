using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Events;

namespace Tauron.Application.CQRS.Client.Domain
{
    public interface ISession
    {
        Task<TType> GetAggregate<TType>(Guid id)
            where TType : AggregateRoot;

        IEventPublisher EventPublisher { get; }
    }
}