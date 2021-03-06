﻿namespace Tauron.Application.CQRS.Common.Dto
{
    public sealed class OperationError
    {
        public int Code { get; set; }

        public string? Description { get; set; }

        public OperationError()
        {
            
        }

        public static OperationError Error(int code, string errorDescription)
            => new OperationError { Code = code, Description = errorDescription };
    }
}