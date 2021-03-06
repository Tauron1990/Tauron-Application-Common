﻿using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Specifications.Fluent
{
    public sealed class GenericSpecification<TType>
    {
        public ISpecification Simple(Func<TType, Task<OperationResult>> eval)
        => eval.Simple();
    }
}