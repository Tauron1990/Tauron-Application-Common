using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using FastExpressionCompiler;
using JetBrains.Annotations;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public static class FastCall
    {
        private static readonly ConcurrentDictionary<object, object> Cache = new ConcurrentDictionary<object, object>();

        private static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue?> value)
            where TValue : class
        {
            if(key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return (TValue) Cache.GetOrAdd(key, k => value.Invoke((TKey)k)!);
        }

        [PublicAPI]
        public static object Clear(this object obj) 
            => GetOrAdd(new ClearCacheKey(obj.GetType()), CreateClearer)(obj);

        private static Func<object, object> CreateClearer(ClearCacheKey arg)
        {
            var prop = arg.TargetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
               .Where(p => p.IsDefined(typeof(RemoveAttribute))).ToArray();

            if (prop.Length == 0) return o => o;

            var parameter = Expression.Parameter(typeof(object));
            var castArg = Expression.Parameter(arg.TargetType);
            var returnTarget = Expression.Label(typeof(object));

            var param = new[] {castArg};
            var expressions = new List<Expression>
                              {
                                  Expression.Assign(castArg, Expression.TypeAs(parameter, arg.TargetType))
                              };

            expressions.AddRange(prop.Select(property => Expression.Assign(Expression.Property(castArg, property), Expression.Default(property.PropertyType))));

            expressions.Add(Expression.Return(returnTarget, Expression.TypeAs(castArg, typeof(object))));
            expressions.Add(Expression.Label(returnTarget, parameter));
            var block = Expression.Block(param, expressions);

            return Expression.Lambda<Func<object, object>>(block, true, parameter).CompileFast();
        }

        public static object CastCall(object instance, Type interfaceType, string methodName, params object[] arguments)
        {
            var key = new CastCallKey(methodName, interfaceType);
            var invoker = GetOrAdd(key, callKey =>
                                                  {
                                                      var map = instance.GetType().GetInterfaceMap(interfaceType);
                                                      if (map.TargetMethods == null) return null;

                                                      var methodInfo = map.TargetMethods.FirstOrDefault(m => m.Name == methodName);
                                                      return methodInfo == null ? null : GetMethodInvokerNoChache(methodInfo);
                                                  });

            if (invoker != null) return invoker(instance, arguments);

            Cache.Remove(key, out _);
            throw new InvalidOperationException($"{instance.GetType().Name} : {interfaceType.Name} -- {methodName} was Not Found");
        }

        public static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
            => GetOrAdd(methodInfo, GetMethodInvokerNoChache);

        private static FastInvokeHandler? GetMethodInvokerNoChache(MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType == null) return null;

            var dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new[] {typeof(object), typeof(object[])}, methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            var ps = methodInfo.GetParameters();
            var paramTypes = new Type[ps.Length];
            for (var i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }

            var locals = new LocalBuilder[paramTypes.Length];

            for (var i = 0; i < paramTypes.Length; i++)
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            for (var i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }

            if (!methodInfo.IsStatic)
                il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < paramTypes.Length; i++)
                il.Emit(ps[i].ParameterType.IsByRef ? OpCodes.Ldloca_S : OpCodes.Ldloc, locals[i]);

            il.EmitCall(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (var i = 0; i < paramTypes.Length; i++)
            {
                if (!ps[i].ParameterType.IsByRef) continue;

                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldloc, locals[i]);
                var memberInfo = locals[i].LocalType;
                if (memberInfo != null && memberInfo.IsValueType)
                    il.Emit(OpCodes.Box, locals[i].LocalType);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ret);
            var invoder = (FastInvokeHandler) dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        private static void EmitCastToReference(ILGenerator il, Type type) => il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);

        private static void EmitBoxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType) il.Emit(OpCodes.Box, type);
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
                default:
                    if (value > -129 && value < 128)
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                    else
                        il.Emit(OpCodes.Ldc_I4, value);
                    return;
            }
        }

        public static Func<object?> GetCreator(Type target)
        {
            return target == null ? () => null : GetOrAdd(new CreatorKey(target), ck => GetCreatorNoCache(ck.TargetType));
        }

        private static Func<object> GetCreatorNoCache(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
                return () => FormatterServices.GetSafeUninitializedObject(type);

            var returnLable = Expression.Label(typeof(object));
            var returnExpression = Expression.Label(returnLable, Expression.Constant(null, typeof(object)));

            var expression = Expression.TryCatch(
                Expression.Return(returnLable,
                                  Expression.TypeAs(
                                      Expression.New(constructor),
                                      typeof(object))),
                Expression.Catch(typeof(Exception),
                                 Expression.Return(returnLable,
                                                   Expression.Call(null, typeof(FormatterServices).GetMethod(nameof(FormatterServices.GetSafeUninitializedObject)) ?? throw new InvalidOperationException(),
                                                                   Expression.Constant(type)))));

            return Expression.Lambda<Func<object>>(Expression.Block(expression, returnExpression)).CompileFast();
        }

        private class CastCallKey : IEquatable<CastCallKey>
        {
            private string MethodName { get; }
            private Type TargetType { get; }

            public CastCallKey(string methodName, Type targetType)
            {
                MethodName = methodName;
                TargetType = targetType;
            }

            public bool Equals(CastCallKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return MethodName == other.MethodName && TargetType == other.TargetType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((CastCallKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((MethodName != null ? MethodName.GetHashCode() : 0) * 397) ^ (TargetType != null ? TargetType.GetHashCode() : 0);
                }
            }
        }

        private class ClearCacheKey : IEquatable<ClearCacheKey>
        {
            private readonly int Identifier = 100;

            public Type TargetType { get; }

            public ClearCacheKey(Type targetType) 
                => TargetType = targetType;

            public bool Equals(ClearCacheKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Identifier == other.Identifier && TargetType == other.TargetType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((ClearCacheKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Identifier * 397) ^ TargetType.GetHashCode();
                }
            }
        }

        private class CreatorKey : IEquatable<CreatorKey>
        {
            private readonly int Identifier = 200;

            public Type TargetType { get; }

            public CreatorKey(Type targetType) 
                => TargetType = targetType;

            public bool Equals(CreatorKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Identifier == other.Identifier && TargetType == other.TargetType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((CreatorKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Identifier * 397) ^ TargetType.GetHashCode();
                }
            }
        }
    }

    public delegate object FastInvokeHandler(object target, params object[] paramters);
}