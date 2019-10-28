using Tauron.Application.CQRS.Client.Specifications.Fluent;

namespace Tauron.Application.CQRS.Client.Specifications
{
    public interface ISpecProvider<TMessage>
    {
        ISpecification Get(GenericSpecification<TMessage> config);
    }
}