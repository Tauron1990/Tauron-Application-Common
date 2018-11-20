using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The ImportSelectorChain interface.</summary>
    [PublicAPI]
    public interface IImportSelectorChain : IImportSelector
    {
        void Register(IImportSelector selector);
    }
}