using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using Tauron.Application.Composition;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public class PropertyModelExtension : IContainerExtension
    {
        public const string EnablePropertyInheritanceMetadataName = "EnablePropertyInheritance";

        private static class TypeInitializationWatcher
        {
            private static readonly List<Type> RunnedTypes = new List<Type>();

            public static void RunStaticConstructor([NotNull] Type constructor)
            {
                
                if (RunnedTypes.Contains(constructor)) return;
                ConstructorInfo init = constructor.TypeInitializer;
                if(init == null) return;

                init.Invoke(null, null);

                RunnedTypes.Add(constructor);
            }
        }

        private class ObservablePropertyTypeCoributor : ITypeContributor
        {
            private class PropertyMemberEmitter : IMemberEmitter
            {
                private readonly PropertyInfo _info;

                public PropertyMemberEmitter([NotNull] PropertyInfo info)
                {
                    _info = info;
                }

                public void EnsureValidCodeBlock()
                {
                
                }

                public void Generate()
                {
                    
                }

                [NotNull]
                public MemberInfo Member { get { return _info; } }

                [NotNull]
                public Type ReturnType { get { return _info.PropertyType; } }
            }

            private static readonly MethodInfo CodeBuilderGenerate = typeof (AbstractCodeBuilder).GetMethod("Generate", BindingFlags.Instance | BindingFlags.NonPublic);

            private static readonly MethodInfo GetValueMethod = typeof (ModelBase).GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] {typeof (string)}, null);
            private static readonly MethodInfo SetValueMethod = typeof(ModelBase).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] {typeof (string), typeof (object)}, null);
            private static readonly MethodInfo GetModelBaseMethod = typeof(ViewModelBase).GetMethod("GetModelBase", BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] {typeof (string)}, null);

            private const MethodAttributes GetSetAttr =
            MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            private const string GetMethodName = "get_";
            private const string SetMethodName = "set_";

            private readonly ImportMetadata _metadata;

            public ObservablePropertyTypeCoributor([NotNull] ImportMetadata metadata)
            {
                _metadata = metadata;
            }

            public void CollectElementsToProxy([NotNull] IProxyGenerationHook hook, [NotNull] MetaType model)
            {
            }

            public void Generate([NotNull] ClassEmitter @class, [NotNull] ProxyGenerationOptions options)
            {
                var exp = CompositionServices.Container.FindExport(_metadata.InterfaceType, _metadata.ContractName, new ErrorTracer());
                
                TypeInitializationWatcher.RunStaticConstructor(exp.Export.ImplementType);

                ObservableProperty[] properties = ModelBase.GetProperties(exp.Export.ImplementType).ToArray();

                foreach (var oProperty in properties)
                {
                    var property = @class.TypeBuilder.DefineProperty(oProperty.Name, PropertyAttributes.None,
                                                                     oProperty.Type, null);

                    var getter = @class.TypeBuilder.DefineMethod(GetMethodName + oProperty.Name, GetSetAttr,
                                                                 oProperty.Type, Type.EmptyTypes);

                    property.SetGetMethod(getter);

                    ILGenerator getterIl = getter.GetILGenerator();
                    var getCodeBuilder = new MethodCodeBuilder(getterIl);
                    GenerateGetter(getCodeBuilder, oProperty);

                    var memberemitter = new PropertyMemberEmitter(property);
                    CodeBuilderGenerate.Invoke(getCodeBuilder, memberemitter, getterIl);

                    if (oProperty.Metadata.IsReadOnly) return;

                    var setter = @class.TypeBuilder.DefineMethod(SetMethodName + oProperty.Name, GetSetAttr, null,
                                                                 new[] {oProperty.Type});
                    property.SetSetMethod(setter);

                    var setterIl = setter.GetILGenerator();
                    var setCodeBuilder = new MethodCodeBuilder(setterIl);
                    GenerateSetter(setCodeBuilder, oProperty);
                    CodeBuilderGenerate.Invoke(setCodeBuilder, memberemitter, setterIl);
                }
            }

            private void GenerateSetter([NotNull] MethodCodeBuilder setCodeBuilder,
                [NotNull] ObservableProperty oProperty)
            {
                setCodeBuilder.AddStatement(
                    new ReturnStatement(new MethodInvocationExpression(GenerateGetModelBase(setCodeBuilder),
                        SetValueMethod,
                        new ReferenceExpression(new ConstReference(oProperty.Name)),
                        new ReferenceExpression(new ArgumentReference(oProperty.Type, 0)))));
            }

            private void GenerateGetter([NotNull] MethodCodeBuilder getCodeBuilder,
                [NotNull] ObservableProperty oProperty)
            {
                getCodeBuilder.AddStatement(
                    new ReturnStatement(new MethodInvocationExpression(GenerateGetModelBase(getCodeBuilder),
                        GetValueMethod,
                        new ReferenceExpression(new ConstReference(oProperty.Name)))));
            }


            [NotNull]
            private Reference GenerateGetModelBase([NotNull] MethodCodeBuilder builder)
            {
                var local = builder.DeclareLocal(typeof (ModelBase));

                builder.AddStatement(new AssignStatement(local,
                    new MethodInvocationExpression(GetModelBaseMethod,
                        new ReferenceExpression(new ConstReference(BuildImportName(_metadata))))));

                return local;
            }
        }
        private class InternalClassProxyGenerator : ClassProxyGenerator
        {
            private readonly ImportMetadata[] _metadatas;

            public InternalClassProxyGenerator([NotNull] ModuleScope scope, [NotNull] Type targetType, [NotNull] ImportMetadata[] metadatas) : base(scope, targetType)
            {
                _metadatas = metadatas;
            }

            [NotNull]
            protected override IEnumerable<Type> GetTypeImplementerMapping([NotNull] Type[] interfaces,
                out IEnumerable<ITypeContributor> contributors, [NotNull] INamingScope namingScope)
            {
                IEnumerable<ITypeContributor> cont;
                var temp = base.GetTypeImplementerMapping(interfaces, out cont, namingScope);

                var realCont = new List<ITypeContributor>(cont);
                realCont.AddRange(
                    _metadatas.Select(importMetadata => new ObservablePropertyTypeCoributor(importMetadata)));

                contributors = realCont;
                return temp;
            }
        }
        private class InternalProxyBuilder : IProxyBuilder
        {
            private readonly ModuleScope _scope;
            private readonly ImportMetadata[] _metadatas;
            private ILogger _logger = NullLogger.Instance;

            [CanBeNull]
            public ILogger Logger
            {
                get
                {
                    return _logger;
                }
                set { _logger = value; }
            }

            [NotNull]
            public ModuleScope ModuleScope
            {
                get
                {
                    return _scope;
                }
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="T:Castle.DynamicProxy.DefaultProxyBuilder" /> class.
            /// </summary>
            /// <param name="scope">The module scope for generated proxy types.</param>
            /// <param name="metadatas"></param>
            public InternalProxyBuilder([NotNull] ModuleScope scope, [NotNull] ImportMetadata[] metadatas)
            {
                _scope = scope;
                _metadatas = metadatas;
            }

            [NotNull]
            public Type CreateClassProxyType([NotNull] Type classToProxy, [NotNull] Type[] additionalInterfacesToProxy,
                [NotNull] ProxyGenerationOptions options)
            {
                AssertValidType(classToProxy);
                AssertValidTypes(additionalInterfacesToProxy);
                var classProxyGenerator = new InternalClassProxyGenerator(_scope, classToProxy, _metadatas)
                {
                    Logger = _logger
                };
                return classProxyGenerator.GenerateCode(additionalInterfacesToProxy, options);
            }

            [NotNull]
            public Type CreateClassProxyTypeWithTarget([NotNull] Type classToProxy,
                [NotNull] Type[] additionalInterfacesToProxy, [NotNull] ProxyGenerationOptions options)
            {
                AssertValidType(classToProxy);
                AssertValidTypes(additionalInterfacesToProxy);
                var classProxyWithTargetGenerator =
                    new ClassProxyWithTargetGenerator(_scope, classToProxy, additionalInterfacesToProxy, options)
                    {
                        Logger = _logger
                    };
                return classProxyWithTargetGenerator.GetGeneratedType();
            }

            [NotNull]
            public Type CreateInterfaceProxyTypeWithTarget([NotNull] Type interfaceToProxy,
                [NotNull] Type[] additionalInterfacesToProxy, [NotNull] Type targetType,
                [NotNull] ProxyGenerationOptions options)
            {
                AssertValidType(interfaceToProxy);
                AssertValidTypes(additionalInterfacesToProxy);
                var interfaceProxyWithTargetGenerator =
                    new InterfaceProxyWithTargetGenerator(_scope, interfaceToProxy)
                    {
                        Logger = _logger
                    };
                return interfaceProxyWithTargetGenerator.GenerateCode(targetType, additionalInterfacesToProxy, options);
            }

            [NotNull]
            public Type CreateInterfaceProxyTypeWithTargetInterface([NotNull] Type interfaceToProxy,
                [NotNull] Type[] additionalInterfacesToProxy, [NotNull] ProxyGenerationOptions options)
            {
                AssertValidType(interfaceToProxy);
                AssertValidTypes(additionalInterfacesToProxy);
                var interfaceProxyWithTargetInterfaceGenerator =
                    new InterfaceProxyWithTargetInterfaceGenerator(_scope, interfaceToProxy)
                    {
                        Logger = _logger
                    };
                return interfaceProxyWithTargetInterfaceGenerator.GenerateCode(interfaceToProxy,
                    additionalInterfacesToProxy, options);
            }

            [NotNull]
            public Type CreateInterfaceProxyTypeWithoutTarget([NotNull] Type interfaceToProxy,
                [NotNull] Type[] additionalInterfacesToProxy, [NotNull] ProxyGenerationOptions options)
            {
                AssertValidType(interfaceToProxy);
                AssertValidTypes(additionalInterfacesToProxy);
                var interfaceProxyWithoutTargetGenerator =
                    new InterfaceProxyWithoutTargetGenerator(_scope, interfaceToProxy)
                    {
                        Logger = _logger
                    };
                return interfaceProxyWithoutTargetGenerator.GenerateCode(typeof (object), additionalInterfacesToProxy,
                    options);
            }

            private static void AssertValidType([NotNull] Type target)
            {
                if (target.IsGenericTypeDefinition)
                {
                    throw new GeneratorException("Type " + target.FullName +
                                                 " is a generic type definition. Can not create proxy for open generic types.");
                }
                if (!IsPublic(target) && !IsAccessible(target))
                {
                    throw new GeneratorException(BuildInternalsVisibleMessageForType(target));
                }
            }

            private static void AssertValidTypes([CanBeNull] IEnumerable<Type> targetTypes)
            {
                if (targetTypes == null) return;

                foreach (Type current in targetTypes)
                {
                    AssertValidType(current);
                }
            }

            private static bool IsAccessible([NotNull] Type target)
            {
                return IsInternal(target) && target.Assembly.IsInternalToDynamicProxy();
            }

            private static bool IsPublic([NotNull] Type target)
            {
                return target.IsPublic || target.IsNestedPublic;
            }

            private static bool IsInternal([NotNull] Type target)
            {
                bool isNested = target.IsNested;
                bool flag = isNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
                return (!target.IsVisible && !isNested) || flag;
            }

            [NotNull]
            private static string BuildInternalsVisibleMessageForType([NotNull] Type target)
            {
                Assembly assembly = target.Assembly;
                string text = " not";
                string text2 = "\"DynamicProxyGenAssembly2\"";
                if (!assembly.IsAssemblySigned())
                    return
                        string.Format(
                            "Type {0} is not visible to DynamicProxy. Can not create proxy for types that are not accessible. Make the type public, or internal and mark your assembly with [assembly: InternalsVisibleTo({1})] attribute, because assembly {2} is{3} strong-named.",
                            new object[]
                            {
                                target.FullName,
                                text2,
                                assembly.GetName().Name,
                                text
                            });
                text = "";
                if (ReferencesCastleCore(assembly))
                {
                    text2 = "InternalsVisible.ToDynamicProxyGenAssembly2";
                }
                else
                {
                    text2 = '"' +
                            "DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7" +
                            '"';
                }
                return
                    string.Format(
                        "Type {0} is not visible to DynamicProxy. Can not create proxy for types that are not accessible. Make the type public, or internal and mark your assembly with [assembly: InternalsVisibleTo({1})] attribute, because assembly {2} is{3} strong-named.",
                        new object[]
                        {
                            target.FullName,
                            text2,
                            assembly.GetName().Name,
                            text
                        });
            }

            private static bool ReferencesCastleCore([NotNull] Assembly inspectedAssembly)
            {
                return
                    inspectedAssembly.GetReferencedAssemblies()
                        .Any(r => r.FullName == Assembly.GetExecutingAssembly().FullName);
            }
        }

        private class PropertyImportInterceptor : IImportInterceptor
        {
            private readonly ImportMetadata[] _metadatas;

            public PropertyImportInterceptor([NotNull] ImportMetadata[] metadatas)
            {
                _metadatas = metadatas;
            }

            public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
            {
                if (!_metadatas.Contains(metadata)) return true;

                var viewModel = (ViewModelBase) target;
                var model = value as ModelBase;

                if (model == null) return true;

                viewModel.RegisterInheritedModel(metadata.ContractName, model);

                return true;
            }
        }

        private class InternalProxyService : IProxyService
        {
            private ModuleScope _moduleScope;
            private ProxyGenerator _genericGenerator;

            public InternalProxyService()
            {
                _moduleScope = new ModuleScope();
                _genericGenerator = new ProxyGenerator(new DefaultProxyBuilder(_moduleScope));
            }

            public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
            {
                interceptor = null;
                if (!typeof (ModelBase).IsAssignableFrom(metadata.Export.ImplementType)) return _genericGenerator;

                var targetImports =
                    imports.Where(meta => meta.Metadata.ContainsKey(EnablePropertyInheritanceMetadataName))
                        .Where(m => (bool) m.Metadata[EnablePropertyInheritanceMetadataName])
                        .ToArray();

                if (targetImports.Length == 0) return _genericGenerator;
            
                interceptor = new PropertyImportInterceptor(targetImports);

                return new ProxyGenerator(new InternalProxyBuilder(_moduleScope, targetImports));
            }

            public ProxyGenerator GenericGenerator
            {
                get
                {
                    return _genericGenerator;
                }
            }
        }

        [NotNull]
        private static string BuildImportName([NotNull] ImportMetadata metadata)
        {
            return metadata.InterfaceType + metadata.ContractName;
        }

        public void Initialize(ComponentRegistry components)
        {
            components.Register<IProxyService, InternalProxyService>(true);
        }
    }
}