using Microsoft.AspNetCore.Http;
using Tauron.Application.CQRS.Client.Core;

namespace Tauron.Application.CQRS.Client.Commands
{
    public abstract class CommandHandlerBase
    {
        public ISession Session { get; internal set; }

        public IdGenerator IdGenerator => IdGenerator.Generator;
    }
}