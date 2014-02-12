// The file Factory.cs is part of Tauron.Application.Common.
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
// <copyright file="Factory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The new.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.IO;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The new.</summary>
    [PublicAPI]
    public static class Factory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The object.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TObject">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TObject" />.
        /// </returns>
        [NotNull]
        public static TObject Object<TObject>([NotNull] params object[] args)
            where TObject : class
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Ensures(Contract.Result<TObject>() != null);

            var tracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(typeof (TObject), tracer, new BuildParameter[0], args);
            if(tracer.Exceptional)
                throw new BuildUpException(tracer);

            return (TObject) val;
        }

        /// <summary>
        ///     The object.
        /// </summary>
        /// <param name="targetType">
        ///     The target Type.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TObject">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TObject" />.
        /// </returns>
        [NotNull]
        public static object Object([NotNull] Type targetType, [NotNull] params object[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Ensures(Contract.Result<object>() != null);

            var errorTracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(targetType, errorTracer, new BuildParameter[0], args);
            if(errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);

            return val;
        }

        #endregion
    }
}