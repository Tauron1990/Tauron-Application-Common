using System;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Domain;

namespace Tauron.Application.Services.Client.Core
{
    public interface IRepository
    {
        Task<T> Get<T>(Guid aggregateId) where T : AggregateRoot;

        void Resfresh(AggregateRoot root);
    }
}