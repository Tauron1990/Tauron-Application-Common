using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Domain;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface IRepository
    {
        Task<T> Get<T>(Guid aggregateId) where T : AggregateRoot;

        void Resfresh(AggregateRoot root);
    }
}