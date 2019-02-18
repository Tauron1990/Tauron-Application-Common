using Tauron.Application.Ioc;

namespace Tauron.Application.Common.Windows
{
    public static class WindowsExtensions
    {
        public static ExportResolver AddWindows(this ExportResolver resolver) => resolver.AddAssembly(typeof(WindowsExtensions));
    }
}