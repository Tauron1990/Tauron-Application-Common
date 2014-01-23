// The file ReflectionExtensions.cs is part of Tauron.Application.Common.
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
// <copyright file="ReflectionExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The reflection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The reflection extensions.</summary>
    [PublicAPI]
    public static class ReflectionExtensions
    {
        #region Constants

        /// <summary>The default binding flags.</summary>
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create instance and unwrap.
        /// </summary>
        /// <param name="domain">
        ///     The domain.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue CreateInstanceAndUnwrap<TValue>(this AppDomain domain) where TValue : class
        {
            Contract.Requires<ArgumentNullException>(domain != null, "domain");
            Contract.Ensures(Contract.Result<TValue>() != null);

            Type targetType = typeof (TValue);
            return (TValue) domain.CreateInstanceAndUnwrap(targetType.Assembly.FullName, targetType.FullName);
        }

        /// <summary>
        ///     The create instance and unwrap.
        /// </summary>
        /// <param name="domain">
        ///     The domain.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue CreateInstanceAndUnwrap<TValue>(this AppDomain domain, params object[] args)
            where TValue : class
        {
            Contract.Requires<ArgumentNullException>(domain != null, "domain");
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Ensures(Contract.Result<TValue>() != null);

            Type targetType = typeof (TValue);
            return
                (TValue)
                domain.CreateInstanceAndUnwrap(
                    targetType.Assembly.FullName,
                    targetType.FullName,
                    false,
                    BindingFlags.Default,
                    null,
                    args,
                    null,
                    null);
        }

        /// <summary>
        ///     The find member attributes.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="nonPublic">
        ///     The non public.
        /// </param>
        /// <param name="bindingflags">
        ///     The bindingflags.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull, SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>(
            [NotNull] this Type type,
            bool nonPublic,
            BindingFlags bindingflags) where TAttribute : Attribute
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Ensures(Contract.Result<IEnumerable<Tuple<MemberInfo, TAttribute>>>() != null);

            bindingflags |= BindingFlags.Public;
            if (nonPublic) bindingflags |= BindingFlags.NonPublic;

            if (!Enum.IsDefined(typeof (BindingFlags), BindingFlags.FlattenHierarchy))
            {
                return from mem in type.GetMembers(bindingflags)
                       let attr = mem.GetCustomAttribute<TAttribute>()
                       where attr != null
                       select Tuple.Create(mem, attr);
            }

            return from mem in type.GetHieratichialMembers(bindingflags)
                   let attr = mem.GetCustomAttribute<TAttribute>()
                   where attr != null
                   select Tuple.Create(mem, attr);
        }

        [NotNull]
        public static IEnumerable<MemberInfo> GetHieratichialMembers([NotNull] this Type type, BindingFlags flags)
        {
            Type targetType = type;
            while (targetType != null)
            {
                foreach (MemberInfo mem in targetType.GetMembers(flags)) yield return mem;

                targetType = targetType.BaseType;
            }
        }

        /// <summary>
        ///     The find member attributes.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="nonPublic">
        ///     The non public.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>(
            this Type type,
            bool nonPublic) where TAttribute : Attribute
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Ensures(Contract.Result<IEnumerable<Tuple<MemberInfo, TAttribute>>>() != null);

            return FindMemberAttributes<TAttribute>(
                type,
                nonPublic,
                BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        ///     The get all attributes.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T[]" />.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static T[] GetAllCustomAttributes<T>(this ICustomAttributeProvider member) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
            Contract.Ensures(Contract.ForAll(Contract.Result<T[]>(), mem => mem != null));
            Contract.Ensures(Contract.Result<T[]>() != null);

            return (T[]) member.GetCustomAttributes(typeof (T), true);
        }

        /// <summary>
        ///     The get all attributes.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="object[]" />.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static object[] GetAllCustomAttributes(this ICustomAttributeProvider member, Type type)
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Ensures(Contract.Result<object[]>() != null);

            return member.GetCustomAttributes(type, true);
        }

        /// <summary>
        ///     The get custom attribute.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TAttribute" />.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider)
            where TAttribute : Attribute
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            return GetCustomAttribute<TAttribute>(provider, true);
        }

        /// <summary>
        ///     The get custom attribute.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <param name="inherit">
        ///     The inherit.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TAttribute" />.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit)
            where TAttribute : Attribute
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            object temp = provider.GetCustomAttributes(typeof (TAttribute), inherit).FirstOrDefault();
            if (temp == null) return null;

            return (TAttribute) temp;
        }

        /// <summary>
        ///     The get custom attributes.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <param name="attributeTypes">
        ///     The attribute types.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<object> GetCustomAttributes(
            this ICustomAttributeProvider provider,
            params Type[] attributeTypes)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");
            Contract.Requires<ArgumentNullException>(attributeTypes != null, "attributeTypes");
            Contract.Requires<ArgumentNullException>(
                Contract.ForAll(attributeTypes, type => type != null),
                "attributeTypes");
            Contract.Ensures(Contract.Result<IEnumerable<object>>() != null);

            return attributeTypes.SelectMany(attributeType => provider.GetCustomAttributes(attributeType, false));
        }

        /// <summary>
        ///     The get invoke member.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType GetInvokeMember<TType>(this MemberInfo info, object instance, params object[] parameter)
            where TType : class
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");
            Contract.Requires<ArgumentNullException>(parameter != null, "parameter");

            if (info is PropertyInfo)
            {
                var property = info.CastObj<PropertyInfo>();
                if (parameter.Length == 0) parameter = null;

                return property.GetValue(instance, parameter) as TType;
            }

            if (info is FieldInfo) return info.CastObj<FieldInfo>().GetValue(instance) as TType;

            if (info is MethodBase) return info.As<MethodBase>().Invoke(instance, parameter) as TType;

            return default(TType);
        }

        /// <summary>
        ///     The get method handle.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="RuntimeMethodHandle" />.
        /// </returns>
        public static RuntimeMethodHandle GetMethodHandle(this MethodBase method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            var mi = method as MethodInfo;

            if (mi != null && mi.IsGenericMethod) return mi.GetGenericMethodDefinition().MethodHandle;

            return method.MethodHandle;
        }

        /// <summary>
        ///     The get parameter types.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<Type> GetParameterTypes(this MethodBase method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

            return method.GetParameters().Select(p => p.ParameterType);
        }

        /// <summary>
        ///     The get property from method.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="implementingType">
        ///     The implementing type.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo" />.
        /// </returns>
        public static PropertyInfo GetPropertyFromMethod(this MethodInfo method, Type implementingType)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");
            Contract.Requires<ArgumentNullException>(implementingType != null, "implementingType");

            if (!method.IsSpecialName || method.Name.Length < 4) return null;

            bool isGetMethod = method.Name.Substring(0, 4) == "get_";
            Type returnType = isGetMethod ? method.ReturnType : method.GetParameterTypes().Last();
            IEnumerable<Type> indexerTypes = isGetMethod
                                                 ? method.GetParameterTypes()
                                                 : method.GetParameterTypes().SkipLast(1);

            return implementingType.GetProperty(
                method.Name.Substring(4),
                DefaultBindingFlags,
                null,
                returnType,
                indexerTypes.ToArray(),
                null);
        }

        /// <summary>
        ///     The get property from method.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo" />.
        /// </returns>
        public static PropertyInfo GetPropertyFromMethod(this MethodBase method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            if (!method.IsSpecialName) return null;

            return method.DeclaringType.GetProperty(method.Name.Substring(4), DefaultBindingFlags);
        }

        /// <summary>
        ///     The get set invoke type.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public static Type GetSetInvokeType(this MemberInfo info)
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");
            Contract.Ensures(Contract.Result<Type>() != null);

            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) info).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo) info).GetParameterTypes().First();
                case MemberTypes.Property:
                    return ((PropertyInfo) info).PropertyType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     The has attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");

            return member.IsDefined(typeof (T), true);
        }

        /// <summary>
        ///     The has attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
            Contract.Requires<ArgumentNullException>(type != null, "type");

            return member.IsDefined(type, true);
        }

        /// <summary>
        ///     The has matching attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <param name="attributeToMatch">
        ///     The attribute to match.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static bool HasMatchingAttribute<T>(this ICustomAttributeProvider member, T attributeToMatch)
            where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
            Contract.Requires<ArgumentNullException>(attributeToMatch != null, "attributeToMatch");

            T[] attributes = member.GetAllCustomAttributes<T>();

            if ((attributes == null) || (attributes.Length == 0)) return false;

            return attributes.Any(attribute => attribute.Match(attributeToMatch));
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType Invoke<TType>(this MethodBase method, object instance, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");
            Contract.Requires<ArgumentNullException>(args != null, "args");

            return (TType) method.Invoke(instance, args);
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Invoke(this MethodBase method, object instance, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            method.Invoke(instance, args);
        }

        /// <summary>
        ///     The parse enum.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="TEnum">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEnum" />.
        /// </returns>
        public static TEnum ParseEnum<TEnum>(this string value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");
            Contract.Ensures(Contract.Result<TEnum>() != null);

            return (TEnum) Enum.Parse(typeof (TEnum), value);
        }

        /// <summary>
        ///     The set invoke member.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public static void SetInvokeMember(this MemberInfo info, object instance, params object[] parameter)
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");

            if (info is PropertyInfo)
            {
                var property = info.CastObj<PropertyInfo>();
                object value = null;
                object[] indexes = null;
                if (parameter != null)
                {
                    if (parameter.Length >= 1) value = parameter[0];

                    if (parameter.Length > 1) indexes = parameter.Skip(1).ToArray();
                }

                property.SetValue(instance, value, indexes);
            }
            else if (info is FieldInfo)
            {
                object value = null;
                if (parameter != null) value = parameter.FirstOrDefault();

                info.As<FieldInfo>().SetValue(instance, value);
            }
            else if (info is MethodBase) info.As<MethodBase>().Invoke(instance, parameter);
        }

        /// <summary>
        ///     The parse enum.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="eEnum">
        ///     The e Enum.
        /// </param>
        /// <typeparam name="TEnum">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEnum" />.
        /// </returns>
        public static bool TryParseEnum<TEnum>(this string value, out TEnum eEnum) where TEnum : struct
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");

            return Enum.TryParse(value, out eEnum);
        }

        #endregion

        public static T ParseEnum<T>([NotNull] this string value, bool ignoreCase)
            where T : struct
        {
            T evalue;
            return Enum.TryParse(value, ignoreCase, out evalue) ? evalue : default(T);
        }
    }
}