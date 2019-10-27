using Microsoft.AspNetCore.Http;
using Tauron.Application.Services.Client.Core;

namespace Tauron.Application.Services.Client.Commands
{
    public abstract class CommandHandlerBase
    {
        public ISession Session { get; internal set; }

        public IdGenerator IdGenerator => IdGenerator.Generator;
    }
}