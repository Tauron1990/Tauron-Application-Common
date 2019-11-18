using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    public abstract class HandlerBase
    {
        public abstract Task Handle(IMessage msg, DomainMessage rawMessage);
    }
}