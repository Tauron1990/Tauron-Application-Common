// The file Helper.cs is part of Tauron.Application.Common.
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
// <copyright file="Helper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The helper.</summary>
    [PublicAPI]
    public static class Helper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The map parameters.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<Tuple<Type, string, bool>> MapParameters(MethodBase info)
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");
            Contract.Ensures(Contract.Result<IEnumerable<Tuple<Type, string, bool>>>() != null);

            foreach (ParameterInfo parameterInfo in info.GetParameters())
            {
                var attr = parameterInfo.GetCustomAttribute<InjectAttribute>();
                if (attr == null) yield return Tuple.Create<Type, string, bool>(parameterInfo.ParameterType, null, false);
                else
                {
                    yield return
                        Tuple.Create(attr.Interface ?? parameterInfo.ParameterType, attr.ContractName, attr.Optional);
                }
            }
        }

        /// <summary>
        ///     The write default creation.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="Func" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        [NotNull]
        public static Func<IBuildContext, ProxyGenerator, object> WriteDefaultCreation([NotNull] IBuildContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Ensures(Contract.Result<Func<IBuildContext, ProxyGenerator, object>>() != null);

            Type type = context.Metadata.Export.ImplementType;

            ConstructorInfo[] construcors = type.GetConstructors(AopConstants.DefaultBindingFlags);
            ConstructorInfo constructor = null;
            foreach (ConstructorInfo constructorInfo in
                construcors.Where(constructorInfo => constructorInfo.GetCustomAttribute<InjectAttribute>() != null))
            {
                if (constructor != null) throw new InvalidOperationException("Too manay Constructors");

                constructor = constructorInfo;
            }

            if (constructor == null) constructor = construcors.First(con => con.GetParameters().Length == 0);

            context.ErrorTracer.Phase = "Returning Default Creation for " + context.Metadata;

            return (build, service) =>
            {
                Contract.Requires<ArgumentNullException>(build != null, "build");
                Contract.Requires<ArgumentNullException>(service != null, "service");
                Contract.Ensures(Contract.Result<object>() != null);

                IEnumerable<object> parameters = from parm in MapParameters(constructor)
                                                 select
                                                     build.Container.Resolve(parm.Item1, parm.Item2, parm.Item3, new BuildParameter[0]);

                var policy = build.Policys.Get<InterceptionPolicy>();

                if (policy != null)
                {
                    context.ErrorTracer.Phase = "Creating Direct Proxy for " + context.Metadata;

                    return service.CreateClassProxy(
                        build.ExportType,
                        null,
                        new ProxyGenerationOptions
                        {
                            Selector =
                                new InternalInterceptorSelector
                            ()
                        },
                        parameters.ToArray(),
                        policy.MemberInterceptor.Select(mem => mem.Value)
                              .ToArray());
                }

                return constructor.Invoke(parameters.ToArray());
            };
        }

        #endregion
    }
}