using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class IOBusinessRuleBase<TInput, TOutput> : RuleBase, IIOBusinessRule<TInput, TOutput>, IRuleDescriptor
    {
        public override bool HasResult { get; } = true;

        public virtual TOutput Action(TInput input)
        {
            try
            {
                SetError(null);
                return ActionImpl(input);
            }
            catch (Exception e)
            {
                if (e.IsCriticalApplicationException()) throw;
                SetError(e);
                return default;
            }
        }

        public override object GenericAction(object input) => input == null ? Action(default) : Action((TInput) input);

        public abstract TOutput ActionImpl(TInput input);

        public Type ParameterType { get; } = typeof(TInput);

        public Type ReturnType { get; } = typeof(TOutput);
    }
}