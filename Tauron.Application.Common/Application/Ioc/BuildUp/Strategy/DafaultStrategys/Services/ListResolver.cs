// The file ListResolver.cs is part of Tauron.Application.Common.
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
// <copyright file="ListResolver.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The list resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The list resolver.</summary>
    public class ListResolver : IResolver
    {
        #region Fields

        private readonly IEnumerable<IResolver> resolvers;

        private readonly Type target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ListResolver" /> Klasse.
        /// </summary>
        /// <param name="resolvers">
        ///     The resolvers.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        public ListResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            this.resolvers = resolvers;
            this.target = target;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Create(ErrorTracer errorTracer)
        {
            try
            {
                errorTracer.Phase = "Injecting List for " + target;

                Type closed = InjectorBaseConstants.List.MakeGenericType(target.GenericTypeArguments[0]);
                if (target.IsAssignableFrom(closed))
                {
                    MethodInfo info = closed.GetMethod("Add");

                   var args = resolvers.Select(resolver => resolver.Create(errorTracer)).TakeWhile(vtemp => !errorTracer.Exceptional).ToList();

                    if (errorTracer.Exceptional) return null;

                    object temp = Activator.CreateInstance(closed);

                    foreach (object o in args) info.Invoke(temp, o);

                    return temp;
                }

                errorTracer.Exceptional = true;
                errorTracer.Exception = new InvalidOperationException(target + " is Not Compatible");

                return null;
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion
    }
}