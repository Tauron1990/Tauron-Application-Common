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
        private static Dictionary<PropertyInfo, Func<object, object[], object>> _propertyAccessorCache = new Dictionary<PropertyInfo, Func<object, object[], object>>();
        private static Dictionary<FieldInfo, Func<object, object>> _fieldAccessorCache = new Dictionary<FieldInfo, Func<object, object>>();
        private static Dictionary<MethodBase, Func<object, object[], object>> _methodCache = new Dictionary<MethodBase, Func<object, object[], object>>();
        private static Dictionary<PropertyInfo, Action<object, object[], object>> _propertySetterCache = new Dictionary<PropertyInfo, Action<object, object[], object>>();
        private static Dictionary<FieldInfo, Action<object, object>> _fieldSetterCache = new Dictionary<FieldInfo, Action<object, object>>();

        private static Expression[] CreateArgumentExpressions(ParameterInfo[] paramsInfo, Expression param)
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

            return argsExpressions;
        }
        private static Func<object[], object> GetCreator(ConstructorInfo constructor)
        {
            lock (_creatorCache)
            {
                if (_creatorCache.TryGetValue(constructor, out var func)) return func;

                // Yes, does this constructor take some parameters?
                var paramsInfo = constructor.GetParameters();

                // Create a single param of type object[].
                var param = Expression.Parameter(typeof(object[]), "args");

                if (paramsInfo.Length > 0)
                {


                    // Make a NewExpression that calls the constructor with the args we just created.
                    var newExpression = Expression.New(constructor, CreateArgumentExpressions(paramsInfo, param));

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

        }
        private static Func<object, object[], object> GetPropertyAccessor(PropertyInfo info, Func<IEnumerable<Type>> arguments)
        {
            lock (_propertyAccessorCache)
            {
                if (_propertyAccessorCache.TryGetValue(info, out var invoker)) return invoker;

                var arg = arguments();

                var instParam = Expression.Parameter(typeof(object));
                var argParam = Expression.Parameter(typeof(object[]));

                Expression acess;
                var convert = info.GetGetMethod()?.IsStatic == true 
                    ? null
                    : Expression.Convert(instParam, Argument.CheckResult(info.DeclaringType, nameof(info.DeclaringType)));

                if(!arg.Any())
                    acess = Expression.Property(convert, info);
                else
                    acess = Expression.Property(convert, info, CreateArgumentExpressions(info.GetIndexParameters(), argParam));


                var delExp = Expression.Convert(acess, typeof(object));
                var del = Expression.Lambda<Func<object, object[], object>>(delExp,  instParam, argParam).CompileFast();

                _propertyAccessorCache[info] = del;
                return del;
            }
        }
        private static Func<object, object> GetFieldAccessor(FieldInfo field)
        {
            lock (_fieldAccessorCache)
            {
                if (_fieldAccessorCache.TryGetValue(field, out var accessor)) return accessor;

                var param = Expression.Parameter(typeof(object));

                var del = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(Expression.Field(
                        field.IsStatic
                            ? null
                            : Expression.Convert(param, Argument.CheckResult(field.DeclaringType, nameof(field.DeclaringType))), field), typeof(object)),
                    param).CompileFast();

                _fieldAccessorCache[field] = del;

                return del;
            }
        }
        private static Action<object, object[], object> GetPropertySetter(PropertyInfo info)
        {
            lock (_propertySetterCache)
            {
                if (_propertySetterCache.TryGetValue(info, out var setter)) return setter;

                var instParam = Expression.Parameter(typeof(object));
                var argsParam = Expression.Parameter(typeof(object[]));
                var valueParm = Expression.Parameter(typeof(object));

                var indexes = info.GetIndexParameters();

                var convertInst = Expression.Convert(instParam, Argument.CheckResult(info.DeclaringType, nameof(info.DeclaringType)));
                var convertValue = Expression.Convert(valueParm, info.PropertyType);

                Expression exp = indexes.Length == 0 
                    ? Expression.Assign(Expression.Property(convertInst, info), convertValue) 
                    : Expression.Assign(Expression.Property(convertInst, info, CreateArgumentExpressions(info.GetIndexParameters(), argsParam)), convertValue);

                setter = Expression.Lambda<Action<object, object[], object>>(exp, instParam, argsParam, valueParm).CompileFast();
                _propertySetterCache[info] = setter;

                return setter;
            }
        }
        private static Action<object, object> GetFieldSetter(FieldInfo info)
        {
            lock (_fieldSetterCache)
            {
                if (_fieldSetterCache.TryGetValue(info, out var setter)) return setter;

                var instParam = Expression.Parameter(typeof(object));
                var valueParam = Expression.Parameter(typeof(object));

                var exp = Expression.Assign(
                    Expression.Field(Expression.Convert(instParam, Argument.CheckResult(info.DeclaringType, nameof(info.DeclaringType))), info),
                    Expression.Convert(valueParam, info.FieldType));

                setter = Expression.Lambda<Action<object, object>>(exp, instParam, valueParam).CompileFast();
                _fieldSetterCache[info] = setter;

                return setter;
            }
        }

        public static Func<object, object[], object> GetMethodInvoker(this MethodInfo info, Func<IEnumerable<Type>> arguments)
        {
            lock (_methodCache)
            {
                if (_methodCache.TryGetValue(info, out var accessor)) return accessor;

                var args = arguments().ToArray();

                var instParam = Expression.Parameter(typeof(object));
                var argsParam = Expression.Parameter(typeof(object[]));
                var convert = info.IsStatic ? null : Expression.Convert(instParam, Argument.CheckResult(info.DeclaringType, nameof(info.DeclaringType)));

                Expression targetExpression = args.Length == 0
                    ? Expression.Call(convert, info) 
                    : Expression.Call(convert, info, CreateArgumentExpressions(info.GetParameters(), argsParam));

                if (info.ReturnType == typeof(void))
                {
                    var label = Expression.Label(typeof(object));
                    var labelExpression = Expression.Label(label, Expression.Constant(null, typeof(object)));

                    targetExpression = Expression.Block(
                        Enumerable.Empty<ParameterExpression>(),
                        targetExpression,
                        //Expression.Return(label, Expression.Constant(null), typeof(object)),
                        labelExpression);
                }
                else
                    targetExpression = Expression.Convert(targetExpression, typeof(object));

                accessor = Expression.Lambda<Func<object, object[], object>>(targetExpression, instParam, argsParam).CompileFast();
                _methodCache[info] = accessor;

                return accessor;
            }
        }
        public static Func<object[], object> GetCreator(Type target, Type[] arguments)
        {
            // Get constructor information?
            var constructor = target.GetConstructor(arguments);

            // Is there at least 1?
            if (constructor == null) return null;

            return GetCreator(constructor);
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

            if(parameter == null)
                parameter = new object[0];

            switch (info)
            {
                case PropertyInfo property:
                {
                    return GetPropertyAccessor(property, () => property.GetIndexParameters().Select(pi => pi.ParameterType))(instance, parameter) is TType 
                        ? (TType) GetPropertyAccessor(property, () => property.GetIndexParameters().Select(pi => pi.ParameterType))(instance, parameter) 
                        : default;
                }
                case FieldInfo field:
                    return (TType) GetFieldAccessor(field)(instance);
                case MethodInfo methodInfo:
                    return (TType) GetMethodInvoker(methodInfo, methodInfo.GetParameterTypes)(instance, parameter);
                case ConstructorInfo constructorInfo:
                    return GetCreator(constructorInfo)(parameter) is TType ? (TType) GetCreator(constructorInfo)(parameter) : default;
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
            switch (info)
            {
                case FieldInfo field:
                    return field.FieldType;
                case MethodBase method:
                    return method.GetParameterTypes().Single();
                case PropertyInfo property:
                    return property.PropertyType;
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

        public static TType InvokeFast<TType>([NotNull] this MethodBase method, [CanBeNull] object instance, [NotNull] params object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            switch (method)
            {
                case MethodInfo methodInfo:
                    return (TType) GetMethodInvoker(methodInfo, methodInfo.GetParameterTypes)(instance, args);
                case ConstructorInfo constructorInfo:
                    return (TType) GetCreator(constructorInfo)(args);
                default:
                    throw new ArgumentException(@"Method Not Supported", nameof(method));
            }
        }

        public static void InvokeFast([NotNull] this MethodInfo method, [CanBeNull] object instance, [NotNull] params object[] args)
        {
            Argument.NotNull(method, nameof(method));

            GetMethodInvoker(method, method.GetParameterTypes)(instance, args);
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
            switch (info)
            {
                case PropertyInfo property:
                {
                    object value = null;
                    object[] indexes = null;
                    if (parameter != null)
                    {
                        if (parameter.Length >= 1) value = parameter[0];

                        if (parameter.Length > 1) indexes = parameter.Skip(1).ToArray();
                    }

                    GetPropertySetter(property)(instance, indexes, value);
                    break;
                }
                case FieldInfo field:
                {
                    object value = null;
                    if (parameter != null) value = parameter.FirstOrDefault();

                    GetFieldSetter(field)(instance, value);
                    break;
                }
                case MethodInfo method:
                    method.InvokeFast(instance, parameter ?? Array.Empty<object>());
                    break;
            }
        }

        public static bool TryParseEnum<TEnum>([NotNull] this string value, out TEnum eEnum) where TEnum : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Enum.TryParse(value, out eEnum);
        }

        public static void SetFieldFast(this FieldInfo field, object target, object value) 
            => GetFieldSetter(Argument.NotNull(field, nameof(field)))(target, value);

        public static void SetValueFast(this PropertyInfo info, object target, object value, params object[] index) 
            => GetPropertySetter(Argument.NotNull(info, nameof(info)))(target, index, value);

        public static object FastCreate(this ConstructorInfo info, params object[] parms) 
            => GetCreator(Argument.NotNull(info, nameof(info)))(parms);

        public static object GetValueFast(this PropertyInfo info, object instance, params object[] index)
            => GetPropertyAccessor(Argument.NotNull(info, nameof(info)), () => info.GetIndexParameters().Select(pi => pi.ParameterType))(instance, index);

        public static object GetValueFast(this FieldInfo info, object instance)
            => GetFieldAccessor(Argument.NotNull(info, nameof(info)))(instance);
    }
}