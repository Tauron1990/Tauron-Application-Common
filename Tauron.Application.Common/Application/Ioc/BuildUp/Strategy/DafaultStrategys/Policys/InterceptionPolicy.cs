﻿// The file InterceptionPolicy.cs is part of Tauron.Application.Common.
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
// <copyright file="InterceptionPolicy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The interception policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Castle.DynamicProxy;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The interception policy.</summary>
    public class InterceptionPolicy : IPolicy
    {
        #region Fields

        private readonly List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>> memberInterceptor;

        private InterceptAttribute interceptAttribute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterceptionPolicy" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InterceptionPolicy" /> Klasse.
        ///     Initializes a new instance of the <see cref="InterceptionPolicy" /> class.
        /// </summary>
        public InterceptionPolicy()
        {
            memberInterceptor = new List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the intercept attribute.</summary>
        /// <value>The intercept attribute.</value>
        public InterceptAttribute InterceptAttribute
        {
            get
            {
                Contract.Ensures(Contract.Result<InterceptAttribute>() != null);

                return interceptAttribute;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                interceptAttribute = value;
            }
        }

        /// <summary>Gets the member interceptor.</summary>
        /// <value>The member interceptor.</value>
        public List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>> MemberInterceptor
        {
            get
            {
                Contract.Ensures(
                    Contract.Result<List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>>>() !=
                    null);
                Contract.Ensures(
                    Contract.ForAll(
                        Contract
                            .Result
                            <List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>>>(),
                        mem => mem.Key != null && mem.Value != null));

                return memberInterceptor;
            }
        }

        #endregion
    }
}