// The file LazyResolver.cs is part of Tauron.Application.Common.
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
// <copyright file="LazyResolver.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The lazy resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The lazy resolver.</summary>
    public class LazyResolver : IResolver
    {
        #region Fields

        private readonly IMetadataFactory factory;

        private readonly Type lazy;

        private readonly SimpleResolver resolver;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LazyResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="LazyResolver" /> Klasse.
        /// </summary>
        /// <param name="resolver">
        ///     The resolver.
        /// </param>
        /// <param name="lazy">
        ///     The lazy.
        /// </param>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        public LazyResolver(SimpleResolver resolver, Type lazy, IMetadataFactory factory)
        {
            this.resolver = resolver;
            this.lazy = lazy;
            this.factory = factory;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Create(ErrorTracer errorTracer)
        {
            return CreateLazy(
                lazy,
                factory,
                resolver.Metadata == null ? new Dictionary<string, object>() : resolver.Metadata.Metadata,
                resolver, errorTracer);
        }

        #endregion

        #region Methods

        private static object CreateLazy(
            Type lazytype,
            IMetadataFactory metadataFactory,
            IDictionary<string, object> metadataValue,
            SimpleResolver creator, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(lazytype != null, "lazytype");
            Contract.Requires<ArgumentNullException>(metadataFactory != null, "metadataFactory");
            Contract.Requires<ArgumentNullException>(metadataValue != null, "metadataValue");
            Contract.Requires<ArgumentNullException>(creator != null, "creator");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");
            Contract.Ensures(Contract.Result<object>() != null);

            errorTracer.Phase = "Injecting Lazy For " + lazytype.Name;

            try
            {
                Type openGeneric = lazytype.GetGenericTypeDefinition();

                Type trampolineBase = typeof (LazyTrampoline<>);
                var trampolineGenerics = new Type[1];
                trampolineGenerics[0] = lazytype.GenericTypeArguments[0];

                Type trampoline = trampolineBase.MakeGenericType(trampolineGenerics);

                var trampolineImpl = (LazyTrampolineBase) Activator.CreateInstance(trampoline, creator);
                Type metadata = openGeneric == InjectorBaseConstants.Lazy ? null : lazytype.GenericTypeArguments[1];

                if (metadata == null) return Activator.CreateInstance(lazytype, trampolineImpl.CreateFunc());

                return Activator.CreateInstance(
                    lazytype,
                    trampolineImpl.CreateFunc(),
                    metadataFactory.CreateMetadata(metadata, metadataValue));
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion

        /// <summary>
        ///     The lazy trampoline.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        internal class LazyTrampoline<T> : LazyTrampolineBase
        {
            #region Fields

            /// <summary>The _resolver.</summary>
            private readonly SimpleResolver _resolver;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LazyTrampoline{T}" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LazyTrampoline{T}" /> Klasse.
            ///     Initializes a new instance of the <see cref="LazyTrampoline{T}" /> class.
            /// </summary>
            /// <param name="resolver">
            ///     The resolver.
            /// </param>
            public LazyTrampoline(SimpleResolver resolver)
            {
                Contract.Requires<ArgumentNullException>(resolver != null, "resolver");

                _resolver = resolver;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The create func.</summary>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            public override object CreateFunc()
            {
                return (Func<T>) Create;
            }

            #endregion

            #region Methods

            /// <summary>The create.</summary>
            /// <returns>
            ///     The <see cref="T" />.
            /// </returns>
            private T Create()
            {
                Contract.Requires(_resolver != null);
                Contract.Ensures(Contract.Result<T>() != null);

                return (T) _resolver.Create(new ErrorTracer());
            }

            #endregion
        }

        internal abstract class LazyTrampolineBase
        {
            #region Public Methods and Operators

            /// <summary>The create func.</summary>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            public abstract object CreateFunc();

            #endregion
        }
    }
}