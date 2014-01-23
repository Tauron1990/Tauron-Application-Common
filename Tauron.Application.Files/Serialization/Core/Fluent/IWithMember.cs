using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IWithMember<out TConfig> 
    {
        [NotNull]
        TConfig WithMember([NotNull] string name);

        [NotNull]
        TConfig WithConverter([NotNull] SimpleConverter<string> converter); 
    }
}