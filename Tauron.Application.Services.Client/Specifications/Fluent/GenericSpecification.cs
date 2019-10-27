using System;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Specifications.Fluent
{
    public sealed class GenericSpecification<TType>
    {
        public ISpecification Simple(Func<TType, Task<OperationResult>> eval)
        => eval.Simple();

        public ISpecification Get(Func<ISpecification> factory, string name)
            => SpecificationFactory<TType>.GetSpecification(factory, name);
    }
}