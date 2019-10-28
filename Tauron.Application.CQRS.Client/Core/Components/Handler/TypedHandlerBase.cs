using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    public abstract class TypedHandlerBase<TMessage> : HandlerBase
        where TMessage : IMessage
    {
        public override async Task Handle(IMessage msg, DomainMessage rawMessage)
        {
            if (msg is TMessage typedMessage)
                await HandleInternal(typedMessage, rawMessage);
        }

        protected abstract Task HandleInternal(TMessage msg, DomainMessage rawMessage);
    }
}