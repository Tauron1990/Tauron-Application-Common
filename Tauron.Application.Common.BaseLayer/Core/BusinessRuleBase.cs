using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class BusinessRuleBase : RuleBase, IBusinessRule, IRuleDescriptor
    {
        public override bool HasResult { get; }

        public virtual void Action()
        {
            try
            {
                SetError(null);
                ActionImpl();
            }
            catch (Exception e)
            {
                if (e.IsCriticalApplicationException()) throw;
                SetError(e);
            }
        }

        public override object GenericAction(object input)
        {
            Action();
            return RuleNull.Null;
        }

        public abstract void ActionImpl();

        public Type ParameterType { get; }

        public Type ReturnType { get; }
    }
}