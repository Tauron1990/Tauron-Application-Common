using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Tauron.Application {
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",
        Justification = "Reviewed. Suppression is OK here.")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class CommandTargetAttribute : MemberInfoAttribute
    {
        [CanBeNull]
        public string CanExecuteMember { get; set; }
        
        public CommandTargetAttribute([NotNull] string memberName)
            : base(memberName)
        {
        }

        public CommandTargetAttribute()
            : base(null)
        {
        }
    }
}