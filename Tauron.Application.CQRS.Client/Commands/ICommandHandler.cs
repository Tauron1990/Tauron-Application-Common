﻿using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<OperationResult?> Handle(TCommand command);
    }
}