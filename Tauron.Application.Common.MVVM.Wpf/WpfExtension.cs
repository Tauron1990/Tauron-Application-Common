using Tauron.Application.Ioc;

namespace Tauron.Application
{
    public static class WpfExtension
    {
        public static ExportResolver AddWpf(this ExportResolver provider)
        {
            provider.AddAssembly(typeof(WpfExtension));
            return provider;
        }
    }
}