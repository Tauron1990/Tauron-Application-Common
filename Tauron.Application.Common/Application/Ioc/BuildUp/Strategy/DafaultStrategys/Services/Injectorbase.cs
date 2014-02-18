﻿// The file Injectorbase.cs is part of Tauron.Application.Common.
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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>
    ///     The injectorbase.
    /// </summary>
    /// <typeparam name="TMember">
    /// </typeparam>
    [ContractClass(typeof (InjectorbaseContracts<>))]
    public abstract class Injectorbase<TMember> : MemberInjector
        where TMember : MemberInfo
    {
        private class UionExportMetatdataEqualityComparer : IEqualityComparer<ExportMetadata>
        {
            public bool Equals([NotNull] ExportMetadata x, [NotNull] ExportMetadata y)
            {
                return x.Export.ImplementType == y.Export.ImplementType;
            }

            public int GetHashCode([NotNull] ExportMetadata obj)
            {
                return obj.Export.ImplementType.GetHashCode();
            }
        }

        #region Fields

        private readonly TMember _member;

        private readonly IMetadataFactory _metadataFactory;

        private int _injectLevel;

        private readonly ExportRegistry _buildParameters = new ExportRegistry();
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Injectorbase{TMember}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="Injectorbase{TMember}" /> Klasse.
        ///     Initializes a new instance of the <see cref="Injectorbase{TMember}" /> class.
        /// </summary>
        /// <param name="metadataFactory">
        ///     The metadata factory.
        /// </param>
        /// <param name="member">
        ///     The member.
        /// </param>
        protected Injectorbase([NotNull] IMetadataFactory metadataFactory, [NotNull] TMember member)
        {
            Contract.Requires<ArgumentNullException>(metadataFactory != null, "metadataFactory");
            Contract.Requires<ArgumentNullException>(member != null, "member");

            _metadataFactory = metadataFactory;
            _member = member;
        }

        #endregion

        #region Properties

        /// <summary>The member.</summary>
        /// <value>The member.</value>
        [NotNull]
        protected TMember Member
        {
            get
            {
                Contract.Ensures(Contract.Result<TMember>() != null);

                return _member;
            }
        }

        /// <summary>The get member type.</summary>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        /// <value>The member type.</value>
        [NotNull]
        protected abstract Type MemberType { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="interceptor"></param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        public override void Inject(object target, IContainer container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters)
        {
            try
            {
                errorTracer.Phase = "Creating Resolver for " + target.GetType().Name + "(" + metadata + ")";

                if (parameters != null)
                {
                    foreach (var export in from buildParameter in parameters
                        where buildParameter != null
                        select buildParameter.CreateExport()
                        into export
                        where export != null
                        select export)
                    {
                        _buildParameters.Register(export, int.MaxValue);
                    }
                }

                InterceptorCallback callback = null;

                if (interceptor != null)
                    callback = new ImportInterceptorHelper(interceptor, Member, metadata, target).Intercept;

                object val;
                if (metadata.Metadata.TryGetValue(LevelSpecificInject.LevelMetadata, out val))
                {
                    try
                    {
                        _injectLevel = (int) val;
                    }
                    catch (InvalidCastException)
                    {
                        _injectLevel = int.MaxValue;
                    }
                }
                else _injectLevel = int.MaxValue;

                IResolver resolver;

                Type memberType = MemberType;

                if (memberType.IsGenericType)
                {
                    Type content = memberType.GetGenericTypeDefinition();

                    if (content == typeof (InstanceResolver<,>))
                    {
                        Type metaType = memberType.GenericTypeArguments[1];

                        resolver = CreateSimple(container, metadata, memberType, true, true, metaType,
                            _metadataFactory.CreateMetadata(metaType, metadata.Metadata), callback, errorTracer);
                    }
                    else if (
                        memberType.IsAssignableFrom(
                            InjectorBaseConstants.List.MakeGenericType(
                                memberType
                                    .GetGenericArguments()[0])))
                    {
                        content = memberType.GetGenericArguments()[0];

                        if (content.IsGenericType)
                        {
                            Type lazyTest = content.GetGenericTypeDefinition();
                            if (lazyTest == InjectorBaseConstants.Lazy ||
                                lazyTest == InjectorBaseConstants.LazyWithMetadata)
                            {
                                resolver =
                                    new ListResolver(
                                        FindAllExports(container, metadata, content.GetGenericArguments()[0],
                                            errorTracer)
                                            .Select(
                                                expr =>
                                                    new LazyResolver(
                                                        new SimpleResolver(expr, container, false, null, null, null,
                                                            callback),
                                                        content,
                                                        _metadataFactory)),
                                        memberType);
                            }
                            else
                            {
                                resolver = CreateSimpleListGeneric(container, metadata, memberType, content,
                                    _metadataFactory, callback, errorTracer);
                            }
                        }
                        else
                            resolver = CreateSimpleListGeneric(container, metadata, memberType, content,
                                _metadataFactory, callback, errorTracer);
                    }
                    else if (InjectorBaseConstants.Lazy == content || InjectorBaseConstants.LazyWithMetadata == content)
                    {
                        resolver = new LazyResolver(
                            CreateSimple(container, metadata, memberType, true, false, null, null, callback, errorTracer),
                            memberType,
                            _metadataFactory);
                    }
                    else if (content.IsArray)
                        resolver = CreateSimpleArray(container, metadata, content, _metadataFactory, callback,
                            errorTracer);
                    else
                        resolver = CreateSimple(container, metadata, memberType, true, false, null, null, callback,
                            errorTracer);
                }
                else if (memberType.IsArray)
                    resolver = CreateSimpleArray(container, metadata, memberType, _metadataFactory,
                        callback, errorTracer);
                else
                    resolver = CreateSimple(container, metadata, memberType, false, false, null, null, callback,
                        errorTracer);

                if (errorTracer.Exceptional) 
                    return;

                errorTracer.Phase = "Resolving Import for " + target.GetType().Name;

                var value = resolver.Create(errorTracer);

                if (errorTracer.Exceptional) return;

                Inject(target, value);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected abstract void Inject([NotNull] object target, [CanBeNull] object value);

        [NotNull]
        private SimpleResolver CreateSimple([NotNull] IContainer container, [NotNull] ImportMetadata metadata,
                                                   [NotNull] Type memberType, bool generic, bool isExportFac,
                                                   [CanBeNull] Type exportMetadata, [CanBeNull] object metadataInstance,
                                                   [CanBeNull] InterceptorCallback callback,
                                                   [NotNull] ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(memberType != null, "memberType");

            ExportMetadata exp = FindExport(container, metadata,
                                            generic ? memberType.GetGenericArguments()[0] : memberType, errorTracer);

            if (isExportFac) return new SimpleResolver(exp, container, true, memberType, metadataInstance, exportMetadata, callback);
            return
                new SimpleResolver(
                    exp,
                    container, false, null, null, null, callback);
        }

        [NotNull]
        private static IResolver CreateSimpleListHelper([NotNull] ExportMetadata meta, [NotNull] Type content,
                                                        [NotNull] IContainer container,
                                                        [NotNull] IMetadataFactory metadataFactory,
                                                        [CanBeNull] InterceptorCallback interceptorCallback)
        {
            if (!content.IsGenericType) 
                return new SimpleResolver(meta, container, false, content, null, null, interceptorCallback);

            if (content.GetGenericTypeDefinition() != typeof (InstanceResolver<,>))
                return new SimpleResolver(meta, container, false, content.GenericTypeArguments[0], null,
                                          null, interceptorCallback);

            Type metaType = content.GenericTypeArguments[1];
            object metaObject = metadataFactory.CreateMetadata(metaType, meta.Metadata);

            return new SimpleResolver(meta, container, true, content.GenericTypeArguments[0], metaObject,
                                      metaType, interceptorCallback);
        }


        [NotNull]
        private ListResolver CreateSimpleListGeneric([NotNull] IContainer container,
                                                            [NotNull] ImportMetadata metadata, [NotNull] Type memberType,
                                                            [NotNull] Type content,
                                                            [NotNull] IMetadataFactory metadataFactoryfactory,
                                                            [CanBeNull] InterceptorCallback interceptorCallback,
                                                            [NotNull] ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(memberType != null, "memberType");
            Contract.Requires<ArgumentNullException>(content != null, "content");

            return
                new ListResolver(
                    FindAllExports(container, metadata, content, errorTracer)
                        .Select(
                            expr => CreateSimpleListHelper(expr, content, container, metadataFactoryfactory, interceptorCallback)),
                    memberType);
        }

        [NotNull]
        private IResolver CreateSimpleArray([NotNull] IContainer container, [NotNull] ImportMetadata metadata,
                                                   [NotNull] Type content, [NotNull] IMetadataFactory metadataFactory,
                                                   [CanBeNull] InterceptorCallback callback, 
                                                   [NotNull] ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(content != null, "content");

            content = content.GetElementType();

            return
                new ArrayResolver(
                    FindAllExports(container, metadata, content, errorTracer)
                        .Select(expr => CreateSimpleListHelper(expr, content, container, metadataFactory, callback)),
                    content);
        }

        [NotNull]
        private IEnumerable<ExportMetadata> FindAllExports([NotNull] IContainer container,
            [NotNull] ImportMetadata metadata,
            [NotNull] Type memberType,
            [NotNull] ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(memberType != null, "memberType");

            Type type = metadata.InterfaceType ?? ExtractRealType(memberType);
            string name = metadata.ContractName;

            return
                _buildParameters.FindAll(type, name, new ErrorTracer())
                    .Union(container.FindExports(type, name, errorTracer, _injectLevel),
                        new UionExportMetatdataEqualityComparer());
        }

        [NotNull]
        private ExportMetadata FindExport([NotNull] IContainer container, [NotNull] ImportMetadata metadata,
                                                 [NotNull] Type memberType, [NotNull] ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(memberType != null, "memberType");

            var type = metadata.InterfaceType ?? ExtractRealType(memberType);
            var name = metadata.ContractName;

            var temp = _buildParameters.FindOptional(type, name, new ErrorTracer());
            if (temp != null)
                return temp;

            return container.FindExport(metadata.InterfaceType ?? ExtractRealType(memberType), metadata.ContractName,
                                        errorTracer, metadata.Optional, _injectLevel);
        }

        [NotNull]
        private static Type ExtractRealType([NotNull] Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (InstanceResolver<,>) ? type.GenericTypeArguments[0] : type;
        }

        #endregion
    }

    [ContractClassFor(typeof (Injectorbase<>))]
    internal abstract class InjectorbaseContracts<TMember> : Injectorbase<TMember>
        where TMember : MemberInfo
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectorbaseContracts{TMember}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InjectorbaseContracts{TMember}" /> Klasse.
        /// </summary>
        /// <param name="metadataFactory">
        ///     The metadata factory.
        /// </param>
        /// <param name="member">
        ///     The member.
        /// </param>
        protected InjectorbaseContracts([NotNull] IMetadataFactory metadataFactory, [NotNull] TMember member)
            : base(metadataFactory, member)
        {
        }

        #endregion

        #region Properties

        /// <summary>Gets the member type.</summary>
        protected override Type MemberType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);
                return typeof (Type);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected override void Inject(object target, object value)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");
        }

        #endregion
    }
}