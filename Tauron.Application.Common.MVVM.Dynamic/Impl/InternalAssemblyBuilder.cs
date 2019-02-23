using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Common.MVVM.Dynamic.Impl
{
    public class InternalAssemblyBuilder
    {
        internal static InternalAssemblyBuilder AssemblyBuilderSingleton { get; } = new InternalAssemblyBuilder();

        private static readonly MethodInfo GetError = typeof(Return).GetProperty(nameof(Return.Error))?.GetMethod;
        private static readonly ConstructorInfo CallErrorException = typeof(CallErrorException).GetConstructor(new[] {typeof(ErrorReturn)});
        private static readonly MethodInfo GetResult = typeof(ObjectReturn).GetProperty(nameof(ObjectReturn.Result))?.GetMethod;

        private List<IExport> _toWrap = new List<IExport>();
        private Dictionary<string, Type> _defindTypes;

        private AssemblyBuilder _internalAssemblyBuilder;

        private InternalAssemblyBuilder()
        {
        }

        public void AddType(IExport type) => _toWrap.Add(Argument.NotNull(type, nameof(type)));

        public bool IsBuilded { get; private set; }

        public Type GetProxyType(Type implment) 
            => _defindTypes.TryGetValue(implment.AssemblyQualifiedName ?? string.Empty, out var type) ? type : null;

        public void Build(IContainer container)
        {
            lock (this)
            {
                if(IsBuilded) return;
                if (_toWrap.Count == 0)
                {
                    IsBuilded = true;
                    _toWrap = null;
                    return;
                }

                var ruleFactory = container.Resolve<IRuleFactory>();
                _defindTypes = new Dictionary<string, Type>();

                _internalAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MvvmDynamic"), AssemblyBuilderAccess.Run);
                var module = _internalAssemblyBuilder.DefineDynamicModule("Main");

                foreach (var export in _toWrap)
                {
                    Type target = export.ImplementType;
                    string name = target.Name;

                    var typeBuilder = module.DefineType(name + "DynamicMvvmProxy", TypeAttributes.Sealed | TypeAttributes.Public, target);
                    CreatePassThroughConstructors(typeBuilder, target);

                    List<(string Name, Delegate Del)> toSet = new List<(string Name, Delegate Del)>();

                    var errorHandler = target.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m =>
                    {
                        if (!m.IsDefined(typeof(RuleErrorHandlerAttribute)))
                            return false;

                        if (m.ReturnParameter?.ParameterType != typeof(void)) return false;

                        var parms = m.GetParameters();
                        return parms.Length == 1 && parms[0].ParameterType == typeof(ErrorReturn);

                    });

                    foreach (var method in from methodInfo in target.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        where methodInfo.IsAbstract
                        let attr = methodInfo.GetCustomAttribute<BindRuleAttribute>()
                        where attr != null
                        select new {Method = methodInfo, Name = attr.Name ?? methodInfo.Name, attr.NoThrow})
                    {
                        var realMethod = method.Method;

                        MethodAttributes mattr = MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                        if (realMethod.IsFamily)
                            mattr |= MethodAttributes.Family;
                        else if (realMethod.IsPrivate)
                            mattr |= MethodAttributes.Private;
                        else if (realMethod.IsPublic)
                            mattr |= MethodAttributes.Public;
                        else
                            mattr |= MethodAttributes.Assembly;

                        var methodBuilder = typeBuilder.DefineMethod(realMethod.Name, mattr, CallingConventions.HasThis, realMethod.ReturnType, 
                            realMethod.GetParameters().Select(p => p.ParameterType).ToArray());
                        
                        var del = ruleFactory.CreateDelegate(method.Name);
                        
                        var fieldBuilder = typeBuilder.DefineField("_del_" + realMethod.Name + "_field", del.GetType(), FieldAttributes.Private | FieldAttributes.Static);
                        var il = methodBuilder.GetILGenerator();
                        
                        if(typeof(Return).IsAssignableFrom(realMethod.ReturnType))
                        {
                            var errorLabel = il.DefineLabel();
                            var returnVar = il.DeclareLocal(realMethod.ReturnType);

                            EmitCall(il, fieldBuilder, del, (short)realMethod.GetParameters().Length);

                            il.Emit(OpCodes.Stloc, returnVar);
                            il.Emit(OpCodes.Ldloc, returnVar);
                            il.Emit(OpCodes.Callvirt, GetError);
                            il.Emit(OpCodes.Brfalse, errorLabel);

                            if (errorHandler != null)
                            {
                                var tempvar = il.DeclareLocal(typeof(ErrorReturn));
                                il.Emit(OpCodes.Ldloc, returnVar);
                                il.Emit(OpCodes.Castclass, typeof(ErrorReturn));
                                il.Emit(OpCodes.Stloc, tempvar);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldloc, tempvar);

                                il.Emit(OpCodes.Callvirt, errorHandler);
                            }

                            il.MarkLabel(errorLabel);

                            il.Emit(OpCodes.Ldloc, returnVar);
                            il.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            bool isVoid = realMethod.ReturnType == typeof(void);

                            var returnVar = il.DeclareLocal(typeof(Return));
                            var falseLabel = il.DefineLabel();

                            EmitCall(il, fieldBuilder, del, (short)realMethod.GetParameters().Length);
                            
                            il.Emit(OpCodes.Stloc, returnVar);
                            il.Emit(OpCodes.Ldloc, returnVar);

                            il.Emit(OpCodes.Callvirt, GetError);
                            il.Emit(OpCodes.Brfalse, falseLabel);

                            if (errorHandler != null)
                            {
                                var tempvar = il.DeclareLocal(typeof(ErrorReturn));
                                il.Emit(OpCodes.Ldloc, returnVar);
                                il.Emit(OpCodes.Castclass, typeof(ErrorReturn));
                                il.Emit(OpCodes.Stloc, tempvar);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldloc, tempvar);

                                il.Emit(OpCodes.Callvirt, errorHandler);
                            }


                            if (method.NoThrow)
                            {
                                if (isVoid)
                                {
                                    il.Emit(OpCodes.Ret);
                                }
                               else if (realMethod.ReturnType.IsValueType)
                               {
                                   il.Emit(OpCodes.Newobj, realMethod.ReturnType.GetConstructor(Type.EmptyTypes));
                                   il.Emit(OpCodes.Ret);
                               }
                               else
                                   il.Emit(OpCodes.Ret);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldloc, returnVar);
                                il.Emit(OpCodes.Castclass, typeof(ErrorReturn));
                                il.Emit(OpCodes.Newobj, CallErrorException);
                                il.Emit(OpCodes.Throw);
                            }

                            il.MarkLabel(falseLabel);
                            if (isVoid)
                            {
                                
                                il.Emit(OpCodes.Ret);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldloc, returnVar);
                                il.Emit(OpCodes.Castclass, typeof(ObjectReturn));
                                il.Emit(OpCodes.Callvirt, GetResult);
                                il.Emit(realMethod.ReturnType.IsValueType ? OpCodes.Unbox : OpCodes.Isinst, realMethod.ReturnType);
                                il.Emit(OpCodes.Ret);
                            }
                        }

                        toSet.Add((fieldBuilder.Name, del));
                    }

                    var newType = typeBuilder.CreateTypeInfo();
                    _defindTypes[Argument.CheckResult(target.AssemblyQualifiedName, nameof(target.AssemblyQualifiedName))] = newType;

                    foreach (var (fieldName, @delegate) in toSet)
                        newType.DeclaredFields.First(fi => fi.Name == fieldName).SetValue(null, @delegate);
                }
                
                IsBuilded = true;
                _toWrap = null;
            }
        }

        private void EmitCall(ILGenerator generator, FieldInfo field, Delegate del, short args)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);

            switch (args)
            {
                case 0:
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.Emit(OpCodes.Ldarg_3);
                    
                    for (short i = 4; i < args; i++)
                    {
                        generator.Emit(OpCodes.Ldarg, i);
                    }
                    break;
            }

            generator.Emit(OpCodes.Callvirt, del.GetType().GetMethod("Invoke"));
        }

        /// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
        /// forwards its arguments to the base constructor, and matches the base constructor's signature.
        /// Supports optional values, and custom attributes on constructors and parameters.
        /// Does not support n-ary (variadic) constructors</summary>
        private static void CreatePassThroughConstructors(TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                        parameterBuilder.SetCustomAttribute(attribute);
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                    ctor.SetCustomAttribute(attribute);

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments?.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments?.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments?.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments?.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}