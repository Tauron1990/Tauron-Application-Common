#region

using System.Diagnostics.CodeAnalysis;

#endregion

[assembly:
    SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
        Target = "Tauron.Application.BaseObject.#Tauron.Application.Ioc.IContextHolder.Context",
        Justification = "Not Public")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Scope = "member",
        Target =
            "Tauron.Application.WindowHook.#InvokeFast(System.IntPtr,System.Int32,System.IntPtr,System.IntPtr,System.Boolean&)",
        Justification = "Native Call")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
        Target = "Tauron.Application.BaseObject.#Tauron.Application.Ioc.IContextHolder.Context",
        Justification = "Not Public")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Tauron.Application.Aop", Justification = "{0}")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Scope = "member",
        Target = "Tauron.Application.Ioc.Constants.#DefaultBindingFlags", Justification = "Name of Enum: BindingFlags")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Scope = "type",
        Target = "Tauron.Application.Ioc.ExportAttribute", Justification = "Public Inheritetable Type")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member",
        Target = "Tauron.Application.Ioc.ExportAttribute.#Metadata", Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member",
        Target = "Tauron.Application.Ioc.BuildUp.ConstructorHelper.#MapParameters(System.Reflection.MethodBase)",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
        Target = "Tauron.Application.Ioc.LifeTime.ILifetimeContext.#GetValue()", Justification = "Unkown Runtime")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Scope = "type",
        Target = "Tauron.Application.Ioc.InjectAttribute", Justification = "Public Api")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member",
        Target =
            "Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Injectorbase`1+IFactory.#CreateIEnumerable()",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Scope = "type",
        Target = "Tauron.Application.Ioc.InterceptAttribute", Justification = "Public API")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member",
        Target = "Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.InterceptionPolicy.#MemberInterceptor",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Scope = "member",
        Target = "Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.InterceptionPolicy.#MemberInterceptor",
        Justification = "{0}")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Scope = "type",
        Target = "Tauron.Application.Ioc.BuildUp.Strategy.IPolicy", Justification = "Static Checking")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member",
        Target = "Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.ObjectContextPolicy.#ContextPropertys",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Scope = "member",
        Target = "Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.ObjectContextPolicy.#ContextPropertys",
        Justification = "{0}")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
        MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle", Scope = "member",
        Target =
            "Tauron.Application.ClipboardViewer.#WinProc(System.IntPtr,System.Int32,System.IntPtr,System.IntPtr,System.Boolean&)",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Ioc.LifeTime.ContextManager.#GetContext(System.String,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Ioc.LifeTime.ContextManager.#GetContext(System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.CountdownEventHolder.#.ctor(System.Threading.CountdownEvent)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Aop.Threading.CountdownEventHolder.#.ctor()",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
        MessageId = "System.Reflection.Assembly.LoadFile", Scope = "member",
        Target =
            "Tauron.Application.Ioc.ExportResolver+PathExportProvider.#CreateExports(Tauron.Application.Ioc.BuildUp.Exports.IExportFactory)",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
        MessageId = "System.Reflection.Assembly.LoadFrom", Scope = "member",
        Target =
            "Tauron.Application.Ioc.ExportResolver+PathExportProvider.#Created(System.Object,System.IO.FileSystemEventArgs)",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
        MessageId = "System.Reflection.Assembly.LoadFrom", Scope = "member",
        Target =
            "Tauron.Application.Ioc.ExportResolver+PathExportProvider.#Deleted(System.Object,System.IO.FileSystemEventArgs)",
        Justification = "No Alternative")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Ioc.ExportResolver.#AddPath(System.String,System.String,System.IO.SearchOption,System.Boolean)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Ioc.ExportResolver+PathExportProvider.#CreateExports(Tauron.Application.Ioc.BuildUp.Exports.IExportFactory)",
        Justification = "?")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.IOExtensions.#OpenTextAppend(System.String)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.ManualResetEventHolder.#.ctor(System.Threading.ManualResetEventSlim)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Aop.Threading.ManualResetEventHolder.#.ctor()",
        Justification = "Longger Lifecycle"
    )]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Aop.Threading.ReaderWriterLockHolder.#.ctor()",
        Justification = "Longger Lifecycle"
    )]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.BarrierSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.CountdownEventSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.ManualResetEventSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.MonitorSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.ReaderWriterLockSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Aop.Threading.SemaphoreHolder.#.ctor(System.Threading.SemaphoreSlim)",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target = "Tauron.Application.Aop.Threading.SemaphoreHolder.#.ctor()",
        Justification = "Longger Lifecycle")]
[assembly:
    SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Scope = "member",
        Target =
            "Tauron.Application.Aop.Threading.SemaphoreSourceAttribute.#Register(Tauron.Application.Ioc.LifeTime.ObjectContext,System.Reflection.MemberInfo,System.Object)",
        Justification = "Longger Lifecycle")]