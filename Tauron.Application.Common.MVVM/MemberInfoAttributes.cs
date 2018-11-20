using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class MemberInfoAttribute : Attribute
    {
        protected MemberInfoAttribute([CanBeNull] string memberName) => MemberName = memberName;

        [CanBeNull]
        public string MemberName { get; }
        
        public bool Synchronize { get; set; }
        
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<Tuple<string, MemberInfo>> GetMembers<TAttribute>([NotNull] Type targetType)
            where TAttribute : MemberInfoAttribute
        {
            return targetType.FindMemberAttributes<TAttribute>(true)
                    .Select(attribute => Tuple.Create(attribute.Item2.ProvideMemberName(attribute.Item1), attribute.Item1));
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void InvokeMembers<TAttribute>([NotNull] object instance, [NotNull] string targetMember, [NotNull] params object[] parameters)
            where TAttribute : MemberInfoAttribute
        {
            Argument.NotNull(instance, nameof(instance));
            Argument.NotNull(targetMember, nameof(targetMember));

            foreach (var member in
                GetMembers<TAttribute>(instance.GetType()).Where(member => member.Item1 == targetMember))
                member.Item2.SetInvokeMember(instance, Argument.NotNull(parameters, nameof(parameters)));
        }
        
        [NotNull]
        public virtual string ProvideMemberName([NotNull] MemberInfo info) => MemberName ?? Argument.NotNull(info, nameof(info)).Name;
    }
}