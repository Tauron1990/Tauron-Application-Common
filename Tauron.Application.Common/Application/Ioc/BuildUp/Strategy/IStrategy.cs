// The file IStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="IStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Strategy interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The Strategy interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (StrategyContracts))]
    public interface IStrategy : IInitializeable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        void OnBuild([NotNull] IBuildContext context);

        /// <summary>
        ///     The on create instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        void OnCreateInstance([NotNull] IBuildContext context);

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        void OnPerpare([NotNull] IBuildContext context);

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        void OnPostBuild([NotNull] IBuildContext context);

        #endregion
    }

    [ContractClassFor(typeof (IStrategy))]
    internal abstract class StrategyContracts : IStrategy
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Initialize(ComponentRegistry components)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The on build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void OnBuild(IBuildContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The on create instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void OnCreateInstance(IBuildContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void OnPerpare(IBuildContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void OnPostBuild(IBuildContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            throw new NotImplementedException();
        }

        #endregion
    }
}