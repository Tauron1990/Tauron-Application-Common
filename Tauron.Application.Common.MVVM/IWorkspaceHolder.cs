using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IWorkspaceHolder
    {
        void Register([NotNull] ITabWorkspace workspace);

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        void UnRegister([NotNull] ITabWorkspace workspace);
    }
}