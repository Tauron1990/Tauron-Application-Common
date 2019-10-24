﻿using System.Threading.Tasks;
using Tauron.Application.Services.Client.Core;

namespace Tauron.Application.Services.Client.Events
{
    public interface IEventExecutor<in TEvent>
    where TEvent : class, IMessage
    {
        Task Apply(TEvent eEvent);
    }
}