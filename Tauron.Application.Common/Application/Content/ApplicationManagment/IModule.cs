// The file IModule.cs is part of Tauron.Application.Common.
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
// <copyright file="IModule.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Module interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The Module interface.</summary>
    [ContractClass(typeof (IModuleContracts)), PublicAPI]
    public interface IModule
    {
        int Order { get; }

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        void Initialize([NotNull] CommonApplication application);

        #endregion
    }

    /// <summary>The i module contracts.</summary>
    [ContractClassFor(typeof (IModule))]
    internal abstract class IModuleContracts : IModule
    {
        #region Public Methods and Operators

        public int Order { get; private set; }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        public void Initialize(CommonApplication application)
        {
            Contract.Requires(application != null);
        }

        #endregion
    }
}