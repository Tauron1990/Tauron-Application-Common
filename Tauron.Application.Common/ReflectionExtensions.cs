using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class ReflectionExtensions
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        private static Dictionary<ConstructorInfo, Func<object[], object>> _creatorCache = new Dictionary<ConstructorInfo, Func<object[], object>>();

        public static Func<object[], object> GetCreator(Type target, Type[] arguments)
        {
            // Get constructor information?
            var constructor = target.GetConstructor(arguments);

            // Is there at least 1?
            if (constructor == null) return null;

            if (_creatorCache.TryGetValue(constructor, out var func)) return func;

            // Yes, does this constructor take some parameters?
            var paramsInfo = constructor.GetParameters();

            // Create a single param of type object[].
            var param = Expression.Parameter(typeof(object[]), "args");

            if (paramsInfo.Length > 0)
            {
                // Pick each arg from the params array and create a typed expression of them.
                var argsExpressions = new Expression[paramsInfo.Length];

                for (var i = 0; i < paramsInfo.Length; i++)
                {
                    Expression index = Expression.Constant(i);
                    Type paramType = paramsInfo[i].ParameterType;
                    Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                    Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                    argsExpressions[i] = paramCastExp;
                }

                // Make a NewExpression that calls the constructor with the args we just created.
                var newExpression = Expression.New(constructor, argsExpressions);

                // Create a lambda with the NewExpression as body and our param object[] as arg.
                var lambda = Expression.Lambda(typeof(Func<object[], object>), newExpression, param);

                // Compile it
                var compiled = (Func<object[], object>)lambda.CompileFast();

                _creatorCache[constructor] = compiled;

                // Success
                return compiled;
            }
            else
            {
                // Make a NewExpression that calls the constructor with the args we just created.
                var newExpression = Expression.New(constructor);

                // Create a lambda with the NewExpression as body and our param object[] as arg.
                var lambda = Expression.Lambda(typeof(Func<object[], object>), newExpression, param);

                // Compile it
                var compiled = (Func<object[], object>)lambda.CompileFast();

                _creatorCache[constructor] = compiled;
                
                // Success
                return compiled;
            }

        }

        public static object FastCreateInstance(this Type target, params object[] parm) 
            => GetCreator(target, parm.Select(o => o.GetType()).ToArray())(parm);

        public static T ParseEnum<T>([NotNull] this string value, bool ignoreCase)
            where T : struct => Enum.TryParse(value, ignoreCase, out T evalue) ? evalue : default;

        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>(
            [NotNull] this Type type,
            bool nonPublic,
            BindingFlags bindingflags) where TAttribute : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            bindingflags |= BindingFlags.Public;
            if (nonPublic) bindingflags |= BindingFlags.NonPublic;

            if (!Enum.IsDefined(typeof(BindingFlags), BindingFlags.FlattenHierarchy))
            {
                return from mem in type.GetMembers(bindingflags)
                    let attr = CustomAttributeExtensions.GetCustomAttribute<TAttribute>(mem)
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
            var targetType = type;
            while (targetType != null)
            {
                foreach (var mem in targetType.GetMembers(flags)) yield return mem;

                targetType = targetType.BaseType;
            }
        }

        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>([NotNull] this Type type,
            bool nonPublic) where TAttribute : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return FindMemberAttributes<TAttribute>(
                type,
                nonPublic,
                BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static T[] GetAllCustomAttributes<T>([NotNull] this ICustomAttributeProvider member) where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            return (T[]) member.GetCustomAttributes(typeof(T), true);
        }

        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static object[] GetAllCustomAttributes([NotNull] this ICustomAttributeProvider member, [NotNull] Type type)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return member.GetCustomAttributes(type, true);
        }

        [CanBeNull]
        public static TAttribute GetCustomAttribute<TAttribute>([NotNull] this ICustomAttributeProvider provider)
            where TAttribute : Attribute
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            return GetCustomAttribute<TAttribute>(provider, true);
        }

        [CanBeNull]
        public static TAttribute GetCustomAttribute<TAttribute>([NotNull] this ICustomAttributeProvider provider, bool inherit)
            where TAttribute : Attribute
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var temp = provider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();

            return (TAttribute) temp;
        }

        [NotNull]
        public static IEnumerable<object> GetCustomAttributes([NotNull] this ICustomAttributeProvider provider, [NotNull] [ItemNotNull] params Type[] attributeTypes)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            return attributeTypes.SelectMany(attributeType => provider.GetCustomAttributes(attributeType, false));
        }

        public static TType GetInvokeMember<TType>([NotNull] this MemberInfo info, [NotNull] object instance, [CanBeNull] params object[] parameter)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            try
            {
                if (info is PropertyInfo)
                {
                    var property = info.SafeCast<PropertyInfo>();
                    if (parameter != null && parameter.Length == 0) parameter = null;

                    return (TType) property.GetValue(instance, parameter);
                }

                if (info is FieldInfo) return (TType) info.SafeCast<FieldInfo>().GetValue(instance);

                if (info is MethodBase) return (TType) info.SafeCast<MethodBase>().Invoke(instance, parameter);
            }
            catch (InvalidCastException)
            {
            }

            return default;
        }

        public static RuntimeMethodHandle GetMethodHandle([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var mi = method as MethodInfo;

            if (mi != null && mi.IsGenericMethod) return mi.GetGenericMethodDefinition().MethodHandle;

            return method.MethodHandle;
        }

        [NotNull]
        public static IEnumerable<Type> GetParameterTypes([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return method.GetParameters().Select(p => p.ParameterType);
        }

        [CanBeNull]
        public static PropertyInfo GetPropertyFromMethod([NotNull] this MethodInfo method, [NotNull] Type implementingType)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (implementingType == null) throw new ArgumentNullException(nameof(implementingType));
            if (!method.IsSpecialName || method.Name.Length < 4) return null;

            var isGetMethod = method.Name.Substring(0, 4) == "get_";
            var returnType = isGetMethod ? method.ReturnType : method.GetParameterTypes().Last();
            var indexerTypes = isGetMethod
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

        [CanBeNull]
        public static PropertyInfo GetPropertyFromMethod([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return !method.IsSpecialName ? null : method.DeclaringType?.GetProperty(method.Name.Substring(4), DefaultBindingFlags);
        }

        [NotNull]
        public static Type GetSetInvokeType([NotNull] this MemberInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
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

        public static bool HasAttribute<T>([NotNull] this ICustomAttributeProvider member) where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            return member.IsDefined(typeof(T), true);
        }

        public static bool HasAttribute([NotNull] this ICustomAttributeProvider member, [NotNull] Type type)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return member.IsDefined(type, true);
        }

        [System.Diagnostics.Contracts.Pure]
        public static bool HasMatchingAttribute<T>([NotNull] this ICustomAttributeProvider member, [NotNull] T attributeToMatch)
            where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (attributeToMatch == null) throw new ArgumentNullException(nameof(attributeToMatch));
            var attributes = member.GetAllCustomAttributes<T>();

            return attributes.Length != 0 && attributes.Any(attribute => attribute.Match(attributeToMatch));
        }

        public static TType Invoke<TType>([NotNull] this MethodBase method, [NotNull] object instance, [NotNull] params object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            return (TType) method.Invoke(instance, args);
        }

        public static void Invoke([NotNull] this MethodBase method, [NotNull] object instance, [NotNull] params object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            method.Invoke(instance, args);
        }

        public static TEnum ParseEnum<TEnum>([NotNull] this string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (TEnum) Enum.Parse(typeof(TEnum), value);
        }

        public static TEnum TryParseEnum<TEnum>(this string value, TEnum defaultValue)
            where TEnum : struct
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return defaultValue;

                return Enum.TryParse<TEnum>(value, out var e) ? e : defaultValue;
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        public static void SetInvokeMember([NotNull] this MemberInfo info, [NotNull] object instance, [CanBeNull] params object[] parameter)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (info is PropertyInfo)
            {
                var property = info.SafeCast<PropertyInfo>();
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

                info.SafeCast<FieldInfo>().SetValue(instance, value);
            }
            else if (info is MethodBase)
            {
                info.SafeCast<MethodBase>().Invoke(instance, parameter);
            }
        }

        public static bool TryParseEnum<TEnum>([NotNull] this string value, out TEnum eEnum) where TEnum : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Enum.TryParse(value, out eEnum);
        }
    }
}