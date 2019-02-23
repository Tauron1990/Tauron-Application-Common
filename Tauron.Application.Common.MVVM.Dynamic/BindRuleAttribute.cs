using System;

namespace Tauron.Application.Common.MVVM.Dynamic
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BindRuleAttribute : Attribute
    {
        public string Name { get; }

        public bool NoThrow { get; set; }

        public BindRuleAttribute(string name) => Name = name;

        public BindRuleAttribute()
        {
            
        }
    }
}