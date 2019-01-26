using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public sealed class ImportMetadata : IEquatable<ImportMetadata>
    {
        public ImportMetadata([CanBeNull] Type interfaceType, [CanBeNull] string contractName, [NotNull] IExport export, [NotNull] string memberName, bool optional, [NotNull] IDictionary<string, object> metadata)
        {
            InterfaceType = interfaceType;
            ContractName = contractName;
            Export = Argument.NotNull(export, nameof(export));
            MemberName = Argument.NotNull(memberName, nameof(memberName));
            Optional = optional;
            Metadata = Argument.NotNull(metadata, nameof(metadata));
        }

        public string ContractName { get; }
        public IExport Export { get; private set; }
        public Type InterfaceType { get; }
        public string MemberName { get; private set; }
        public IDictionary<string, object> Metadata { get; private set; }
        public bool Optional { get; private set; }

        public bool Equals(ImportMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            bool flag;
            if (InterfaceType != null) flag = InterfaceType == other.InterfaceType;
            else flag = other.InterfaceType == null;

            return flag && string.Equals(ContractName, other.ContractName)
                        && string.Equals(MemberName, other.MemberName);
        }

        public static bool operator ==(ImportMetadata left, ImportMetadata right) => Equals(left, right);

        public static bool operator !=(ImportMetadata left, ImportMetadata right) => !Equals(left, right);

        public override bool Equals(object obj)
        {
            var met = obj as ImportMetadata;
            return met != null && Equals(met);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (InterfaceType?.GetHashCode() * 397 ?? 0)
                       ^ (ContractName?.GetHashCode() ?? 0);
            }
        }

        [DebuggerStepThrough]
        public override string ToString() => ErrorTracer.FormatExport(this);
    }
}