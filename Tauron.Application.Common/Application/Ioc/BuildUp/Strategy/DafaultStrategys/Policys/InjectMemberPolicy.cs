﻿// The file InjectMemberPolicy.cs is part of Tauron.Application.Common.
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
// <copyright file="InjectMemberPolicy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The inject member policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The inject member policy.</summary>
    public class InjectMemberPolicy : IPolicy
    {
        private MemberInjector _injector;

        private ImportMetadata _metadata;

        [NotNull]
        public ImportMetadata Metadata
        {
            get
            {
                Contract.Ensures(Contract.Result<ImportMetadata>() != null);

                return _metadata;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _metadata = value;
            }
        }

        /// <summary>Gets or sets the injector.</summary>
        /// <value>The injector.</value>
        [NotNull]
        public MemberInjector Injector
        {
            get
            {
                Contract.Ensures(Contract.Result<MemberInjector>() != null);

                return _injector;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _injector = value;
            }
        }

        [CanBeNull]
        public List<IImportInterceptor> Interceptors { get; set; }
    }
}