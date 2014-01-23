using System.Collections.Generic;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IIniKeySerializerConfiguration : IWithMember<IIniKeySerializerConfiguration>
    {
        [NotNull]
        IIniKeySerializerConfiguration WithKey([CanBeNull] string name);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "No simpler way"), NotNull]
        IIniKeySerializerConfiguration WithConverter([CanBeNull] SimpleConverter<IEnumerable<string>> converter);

        [NotNull]
        IIniSerializerConfiguration Apply();
    }
}