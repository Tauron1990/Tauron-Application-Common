#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The control target attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [DebuggerNonUserCode]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class ControlTargetAttribute : MemberInfoAttribute
    {
        public ControlTargetAttribute()
            : base(null) {}

        public ControlTargetAttribute([CanBeNull] string memberName)
            : base(memberName) {}

    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [DebuggerNonUserCode]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class WindowTargetAttribute : MemberInfoAttribute
    {
        public WindowTargetAttribute()
            : base(null) {}

        public WindowTargetAttribute([CanBeNull] string memberName)
            : base(memberName) {}

    }

    [PublicAPI]
    public static class ControlHelper
    {
        [DebuggerNonUserCode]
        private class ControlLinker : LinkerBase
        {
            public ControlLinker([NotNull] string name, [NotNull] DependencyObject element)
                : base(name, element){}

            public override void Scan()
            {
                var context = DataContext?.Target;
                if (context == null) return;

                MemberInfoAttribute.InvokeMembers<ControlTargetAttribute>(context, Name, Target);
            }

        }

        private abstract class LinkerBase : PipelineBase
        {
            private string _name;

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            protected LinkerBase([NotNull] string name, [NotNull] DependencyObject element)
                : base(element, false) => Name = Argument.NotNull(name, nameof(name));

            [NotNull]
            public string Name
            {
                get => _name;

                set
                {
                    _name = value;
                    Scan();
                }
            }

            public abstract void Scan();

            protected override void DataContextChanged() => Scan();
        }

        private class WindowLinker : LinkerBase
        {
            public WindowLinker([NotNull] string name, [NotNull] DependencyObject element)
                : base(name, element) { }

            public override void Scan()
            {
                var realName = Name;
                string windowName = null;

                if (realName.Contains(":"))
                {
                    var nameSplit = realName.Split(new[] {':'}, 2);
                    realName = nameSplit[0];
                    windowName = nameSplit[1];
                }

                object context;
                var priTarget = Target;
                if (DataContext == null || (context = DataContext.Target) == null || priTarget == null)
                {
                    CommonConstants.LogCommon(false, "ControlHelper: No Context Found");
                    return;
                }

                if (windowName == null)
                {
                    if (!(priTarget is Window)) priTarget = Window.GetWindow(priTarget);

                    if (priTarget == null)
                        CommonWpfConstans.LogCommon(false, "ControlHelper: No Window Found: {0}|{1}", context.GetType(), realName);
                }
                else
                {
                    priTarget =
                        System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault(win => win.Name == windowName);

                    if (priTarget == null)
                        CommonWpfConstans.LogCommon(false, "ControlHelper: No Window Named {0} Found", windowName);
                }

                if (priTarget == null) return;

                foreach (var member in MemberInfoAttribute.GetMembers<WindowTargetAttribute>(context.GetType())
                        .Where(mem => mem.Item1 == realName))
                    try
                    {
                        var targetType = member.Item2.GetSetInvokeType();

                        object arg;
                        if (targetType == typeof(IWindow)) arg = new WpfWindow((Window) priTarget);
                        else arg = priTarget;

                        member.Item2.SetInvokeMember(context, arg);
                    }
                    catch (Exception e)
                    {
                        CommonConstants.LogCommon(true, "ControlHelper: Error On {0} Member Acess: {1}", member.Item2.Name, e);

                        throw;
                    }
            }

        }

        public static readonly DependencyProperty MarkControlProperty =
            DependencyProperty.RegisterAttached("MarkControl", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(string.Empty, MarkControl));

        public static readonly DependencyProperty MarkWindowProperty = 
            DependencyProperty.RegisterAttached("MarkWindow", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(null, MarkWindowChanged));

        private static readonly WeakReferenceCollection<LinkerBase> LinkerCollection = new WeakReferenceCollection<LinkerBase>();

        [NotNull]
        public static string GetMarkControl([NotNull] DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(MarkControlProperty);

        [NotNull]
        public static string GetMarkWindow([NotNull] DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(MarkWindowProperty);

        public static void SetMarkControl([NotNull] DependencyObject obj, [NotNull] string value) 
            => Argument.NotNull(obj, nameof(obj)).SetValue(MarkControlProperty, Argument.NotNull(value, nameof(value)));

        public static void SetMarkWindow([NotNull] DependencyObject obj, [NotNull] string value) 
            => Argument.NotNull(obj, nameof(obj)).SetValue(MarkWindowProperty, Argument.NotNull(value, nameof(value)));

        private static void MarkControl([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLinker(d, e.OldValue.As<string>() ?? throw new InvalidOperationException(), e.NewValue.As<string>() ?? throw new InvalidOperationException(),
                (obj, str) => new ControlLinker(str, obj));
        }

        // Using a DependencyProperty as the backing store for MarkWindow.  This enables animation, styling, binding, etc...
        private static void MarkWindowChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => SetLinker(d, e.OldValue.As<string>(), e.NewValue.As<string>(), (obj, str) => new WindowLinker(str, obj));

        private static void SetLinker([NotNull] DependencyObject obj, [CanBeNull] string oldName, [CanBeNull] string newName,
            [NotNull] Func<DependencyObject, string, LinkerBase> factory)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return;

            Argument.NotNull(obj, nameof(obj));
            Argument.NotNull(factory, nameof(factory));
            if (DesignerProperties.GetIsInDesignMode(obj)) return;

            foreach (var linker in LinkerCollection.Where(linker => Equals(linker.Target, obj) && linker.Name == oldName))
            {
                linker.Name = newName;
                return;
            }

            var pipline = factory(obj, newName);
            pipline.Scan();
            LinkerCollection.Add(pipline);
        }
    }
}