// The file ContextScopeAttribute.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextScopeAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The context scope attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The context scope attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ContextScopeAttribute : ExportMetadataBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextScopeAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ContextScopeAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ContextScopeAttribute" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        public ContextScopeAttribute(string name)
            : base(AopConstants.ContextMetadataName, name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextScopeAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ContextScopeAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ContextScopeAttribute" /> class.
        /// </summary>
        public ContextScopeAttribute()
            : base(AopConstants.ContextMetadataName, null)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return InternalValue as string; }
        }

        #endregion
    }
}