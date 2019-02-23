using System;
using Tauron.Application.Common.MVVM.Dynamic.Impl;

namespace Tauron.Application.Common.MVVM.Dynamic
{
    [ExportModule]
    public class DynamicModule : IModule
    {
        public int Order { get; } = 0;

        public void Initialize(CommonApplication application, Action<ComponentUpdate> addComponent)
        {
            addComponent(new ComponentUpdate("Dynamic Types"));

            InternalAssemblyBuilder.AssemblyBuilderSingleton.Build(application.Container);
        }
    }
}