// The file ContextPropertyAttributeBase.cs is part of Tauron.Application.Common.
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
// <copyright file="ContextPropertyAttributeBase.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The context property attribute base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The context property attribute base.</summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ContextPropertyAttributeBase : ObjectContextPropertyAttribute
    {
        #region Fields

        private string _holderName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ContextPropertyAttributeBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        /// </summary>
        protected ContextPropertyAttributeBase()
        {
            HolderName = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the holder name.</summary>
        /// <value>The holder name.</value>
        [NotNull]
        public string HolderName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _holderName;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _holderName = value;
            }
        }

        #endregion
    }
}