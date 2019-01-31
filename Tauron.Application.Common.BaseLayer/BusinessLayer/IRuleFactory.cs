using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer {
    public interface IRuleFactory {
        IRuleBase Create(string name);
        IBusinessRule CreateBusinessRule(string name);
        IIBusinessRule<TType> CreateIiBusinessRule<TType>(string name);

        IIOBusinessRule<TInput, TOutput> CreateIioBusinessRule<TInput, TOutput>(string name)
            //where TOutput : class where TInput : class
            ;

        IOBussinesRule<TOutput> CreateOBussinesRule<TOutput>(string name)
            //where TOutput : class
            ;

        CompositeRule<TInput, TOutput> CreateComposite<TInput, TOutput>(params string[] names);
    }
}