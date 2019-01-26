using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    [Serializable]
    public sealed class ExportMetadata : IEquatable<ExportMetadata>
    {

        public ExportMetadata([NotNull] Type interfaceType, [CanBeNull] string contractName, [NotNull] Type lifetime,
            [NotNull] Dictionary<string, object> metadata, [NotNull] IExport export)
        {
            InterfaceType = Argument.NotNull(interfaceType, nameof(interfaceType));
            ContractName = contractName;
            Lifetime = Argument.NotNull(lifetime, nameof(lifetime));
            Metadata = Argument.NotNull(metadata, nameof(metadata));
            Export = Argument.NotNull(export, nameof(export));
            InterfaceName = interfaceType?.Name;
        }

        [CanBeNull]
        public string ContractName { get; }

        [NotNull]
        public IExport Export { get; set; }

        [NotNull]
        public Type InterfaceType { get; }

        [NotNull]
        public Type Lifetime { get; }

        [CanBeNull]
        public Dictionary<string, object> Metadata { get; private set; }

        public string InterfaceName { get; }

        public bool Equals(ExportMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return InterfaceType == other.InterfaceType && string.Equals(ContractName, other.ContractName)
                                                        && Lifetime == other.Lifetime && Export.Equals(other.Export);
        }

        public static bool operator ==(ExportMetadata left, ExportMetadata right) => Equals(left, right);

        public static bool operator !=(ExportMetadata left, ExportMetadata right) => !Equals(left, right);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var meta = obj as ExportMetadata;

            return meta != null && Equals(meta);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InterfaceType.GetHashCode();
                hashCode = (hashCode * 397) ^ (ContractName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Lifetime.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (Metadata == null || !Metadata.TryGetValue("DebugName", out var name)) name = ContractName;
            return ErrorTracer.FormatExport(this); //InterfaceType, name);
        }
    }
}