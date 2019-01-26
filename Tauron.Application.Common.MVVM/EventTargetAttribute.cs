using System;
using JetBrains.Annotations;

namespace Tauron.Application {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class EventTargetAttribute : MemberInfoAttribute
    {
        public Type Converter { get; set; }

        public EventTargetAttribute([CanBeNull] string memberName)
            : base(memberName){}

        public EventTargetAttribute()
            : base(null){}

    }
}