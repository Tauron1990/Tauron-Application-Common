// The file MemberInfoAttributes.cs is part of Tauron.Application.Common.Wpf.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberInfoAttributes.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The member info attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The member info attribute.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class MemberInfoAttribute : Attribute
    {
        #region Fields

        private readonly string _memberName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberInfoAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MemberInfoAttribute" /> Klasse. Initialisiert eine neue Instanz der
        ///     Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     Der Name des Members.
        /// </param>
        protected MemberInfoAttribute([CanBeNull] string memberName)
        {
            _memberName = memberName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Der Name des Kommandos
        /// </summary>
        [CanBeNull]
        public string MemberName
        {
            get { return _memberName; }
        }

        /// <summary>Gets or sets a value indicating whether synchronize.</summary>
        public bool Synchronize { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get members.
        /// </summary>
        /// <param name="targetType">
        ///     The target type.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        [NotNull,SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<Tuple<string, MemberInfo>> GetMembers<TAttribute>([NotNull] Type targetType)
            where TAttribute : MemberInfoAttribute
        {
            return
                targetType.FindMemberAttributes<TAttribute>(true)
                          .Select(
                              attribute =>
                              Tuple.Create(attribute.Item2.ProvideMemberName(attribute.Item1), attribute.Item1));
        }

        /// <summary>
        ///     The invoke members.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="targetMember">
        ///     The target member.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void InvokeMembers<TAttribute>([NotNull] object instance, [NotNull] string targetMember, [NotNull] params object[] parameters)
            where TAttribute : MemberInfoAttribute
        {
            Contract.Requires<ArgumentNullException>(instance != null, "instance");

            foreach (var member in
                GetMembers<TAttribute>(instance.GetType()).Where(member => member.Item1 == targetMember)) member.Item2.SetInvokeMember(instance, parameters);
        }

        /// <summary>
        ///     The provide member name.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public virtual string ProvideMemberName([NotNull] MemberInfo info)
        {
            Contract.Requires<ArgumentNullException>(info != null, "instance");

            return MemberName ?? info.Name;
        }

        #endregion
    }
}