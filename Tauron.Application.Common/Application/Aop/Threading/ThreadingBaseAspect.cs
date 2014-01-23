// The file ThreadingBaseAspect.cs is part of Tauron.Application.Common.
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
// <copyright file="ThreadingBaseAspect.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The threading base aspect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The threading base aspect.</summary>
    public abstract class ThreadingBaseAspect : AspectBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadingBaseAspect" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ThreadingBaseAspect" /> Klasse.
        ///     Initializes a new instance of the <see cref="ThreadingBaseAspect" /> class.
        /// </summary>
        protected ThreadingBaseAspect()
        {
            Order = 200;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the holder name.</summary>
        /// <value>The holder name.</value>
        [NotNull]
        public string HolderName { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether is initialized.</summary>
        /// <value>The is initialized.</value>
        protected bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        protected internal override void Initialize([NotNull] object target, [NotNull] ObjectContext context, [NotNull] string contextName)
        {
            base.Initialize(target, context, contextName);

            IsInitialized = true;
        }

        #endregion
    }
}