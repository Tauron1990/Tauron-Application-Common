using System;
using Tauron.Application.CQRS.Client.Core;
using Tauron.Application.CQRS.Client.Domain;

namespace Tauron.Application.CQRS.Client.Commands
{
    public abstract class CommandHandlerBase
    {
        private ISession? _session;

        public ISession Session
        {
            get
            {
                if(_session == null)
                    throw new InvalidOperationException("Session is Null");
                return _session;
            }
            internal set => _session = value;
        }

        public IdGenerator IdGenerator => IdGenerator.Generator;
    }
}