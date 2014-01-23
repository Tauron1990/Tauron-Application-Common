// The file ExportAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class ExportAttribute : Attribute
    {
        #region Fields

        private readonly Type m_export;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportAttribute" /> class.
        /// </summary>
        /// <param name="export">
        ///     The export.
        /// </param>
        public ExportAttribute(Type export)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");

            m_export = export;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the contract name.</summary>
        /// <value>The contract name.</value>
        public string ContractName { get; set; }

        /// <summary>Gets the export.</summary>
        /// <value>The export.</value>
        public Type Export
        {
            get { return m_export; }
        }

        /// <summary>The get metadata.</summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        /// <value>The metadata.</value>
        public IEnumerable<Tuple<string, object>> Metadata
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Tuple<string, object>>>() != null);

                if (!HasMetadata) yield break;

                foreach (PropertyInfo property in
                    GetType().GetProperties().Where(property => property.Name != "Metadata")) yield return Tuple.Create(property.Name, property.GetValue(this));
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether has metadata.</summary>
        /// <value>The has metadata.</value>
        protected virtual bool HasMetadata
        {
            get { return false; }
        }

        [CanBeNull]
        protected virtual LifetimeContextAttribute OverrideDefaultPolicy
        {
            get { return null; }
        }

        [CanBeNull]
        internal LifetimeContextAttribute GetOverrideDefaultPolicy()
        {
            return OverrideDefaultPolicy;
        }

        #endregion
    }
}