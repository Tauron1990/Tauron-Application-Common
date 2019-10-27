using Tauron.Application.Services.Client.Specifications.Fluent;

namespace Tauron.Application.Services.Client.Specifications
{
    public interface ISpecProvider<TMessage>
    {
        ISpecification Get(GenericSpecification<TMessage> config);
    }
}