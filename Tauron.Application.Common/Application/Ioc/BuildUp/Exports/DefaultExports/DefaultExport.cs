using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExpressionBuilder;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    [Serializable]
    public sealed class DefaultExport : IExport
    {
        private void Initialize(bool anonym)
        {
            _isAnonymos = anonym;

            Globalmetadata = new Dictionary<string, object>();

            IEnumerable<ExportMetadataBaseAttribute> metadata =
                _attributeProvider.GetAllCustomAttributes<ExportMetadataBaseAttribute>();
            foreach (var exportMetadataAttribute in metadata) Globalmetadata[exportMetadataAttribute.InternalKey] = exportMetadataAttribute.InternalValue;

            var attr = Globalmetadata.TryGetAndCast<LifetimeContextAttribute>(AopConstants.LiftimeMetadataName);

            var lifetime = attr?.LifeTimeType ?? typeof(SharedLifetime);

            if (anonym)
            {
                _exports = new[]
                {
                    new ExportMetadata(
                        _exportetType,
                        ExternalInfo.ExtenalComponentName,
                        typeof(NotSharedLifetime),
                        new Dictionary<string, object>(Globalmetadata),
                        this)
                };
                return;
            }

            _exports = _attributeProvider.GetAllCustomAttributes<ExportAttribute>()
                .Select(
                    attribute =>
                    {
                        var temp = new Dictionary<string, object>(Globalmetadata);

                        Type realLifetime;

                        if (attr == null)
                        {
                            var customLifeTime = attribute.GetOverrideDefaultPolicy();
                            if (customLifeTime != null)
                            {
                                realLifetime = customLifeTime.LifeTimeType;
                                attr = customLifeTime;
                            }
                            else
                            {
                                realLifetime = lifetime;
                            }
                        }
                        else
                        {
                            realLifetime = lifetime;
                        }

                        foreach (var tuple in attribute.Metadata)
                            temp.Add(tuple.Item1,
                                tuple.Item2);

                        return new ExportMetadata(attribute.Export, attribute.ContractName, realLifetime, temp, this);
                    })
                .ToArray();

            ShareLifetime = attr == null || attr.ShareLiftime;
        }

        private readonly ICustomAttributeProvider _attributeProvider;
        private readonly Type _exportetType;
        private ExportMetadata[] _exports;

        private bool _isAnonymos;

        public DefaultExport(object export)
            : this(export?.GetType(), export)
        {
        }

        public DefaultExport(Type exportetType, object obj)
            : this(exportetType, new ExternalExportInfo(true, true, true, false, Operation.Constant, exportetType?.FullName), true)
        {
        }

        public DefaultExport([NotNull] Type exportetType, [NotNull] ExternalExportInfo externalInfo, bool asAnonym)
        {
            Globalmetadata = new Dictionary<string, object>();
            _exportetType = Argument.NotNull(exportetType, nameof(exportetType));
            _attributeProvider = exportetType;
            ExternalInfo = Argument.NotNull(externalInfo, nameof(externalInfo));
            Initialize(asAnonym);
        }

        public DefaultExport([NotNull] MethodInfo info, [NotNull] ExternalExportInfo externalInfo, bool asAnonym)
        {
            Globalmetadata = new Dictionary<string, object>();
            _attributeProvider = Argument.NotNull(info, nameof(info));
            ExternalInfo = Argument.NotNull(externalInfo, nameof(externalInfo));
            Initialize(asAnonym);
        }

        [NotNull]
        public IEnumerable<ExportMetadata> ExportMetadata => _exports;

        [NotNull]
        public IEnumerable<Type> Exports => _exports.Select(ex => ex.InterfaceType);

        [NotNull]
        public ExternalExportInfo ExternalInfo { get; private set; }
        public Dictionary<string, object> Globalmetadata { get; private set; }
        public Type ImplementType => _exportetType;
        public IEnumerable<ImportMetadata> ImportMetadata { get; internal set; }
        public bool ShareLifetime { get; private set; }

        public bool Equals(IExport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return ImplementType == other.ImplementType;
        }

        public ExportMetadata GetNamedExportMetadata(string contractName) => _isAnonymos ? _exports[0] : _exports.Single(exm => exm.ContractName == contractName);
        public IEnumerable<ExportMetadata> SelectContractName(string contractName) => _isAnonymos ? _exports : _exports.Where(meta => meta.ContractName == contractName);

        public static bool IsExport([NotNull] ICustomAttributeProvider type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.GetCustomAttributes(typeof(ExportAttribute), false).Length != 0;
        }

        public static bool operator ==(DefaultExport left, DefaultExport right) => Equals(left, right);
        public static bool operator !=(DefaultExport left, DefaultExport right) => !Equals(left, right);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is DefaultExport export && Equals(export);
        }

        public override int GetHashCode() => _attributeProvider.GetHashCode();

        public override string ToString() => ImplementType?.ToString() ?? string.Empty;
    }
}