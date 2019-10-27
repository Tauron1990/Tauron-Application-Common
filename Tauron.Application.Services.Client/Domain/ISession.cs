using System;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Events;

namespace Tauron.Application.Services.Client.Domain
{
    public interface ISession
    {
        Task<TType> GetAggregate<TType>(Guid id)
            where TType : AggregateRoot;

        IEventPublisher EventPublisher { get; }
    }
}