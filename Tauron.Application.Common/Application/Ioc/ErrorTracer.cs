using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc
{
    public sealed class ErrorTracer : MarshalByRefObject
    {
        private readonly StringBuilder _internalPhase = new StringBuilder();
        private int _ident;

        public bool Exceptional { get; set; }

        [NotNull]
        public string Phase
        {
            get => _internalPhase.ToString();
            set
            {
                for (var i = 0; i < _ident; i++) _internalPhase.Append('\t');

                _internalPhase.AppendLine(value);
            }
        }

        [CanBeNull]
        public string Export { get; set; }

        [CanBeNull]
        public Exception Exception { get; set; }

        public void IncrementIdent()
        {
            _ident++;
        }

        public void DecrementIdent()
        {
            _ident--;
        }

        [NotNull]
        [DebuggerStepThrough]
        public static string FormatExport(ExportMetadata meta) 
            => meta.ContractName == null 
                ? $"{meta.InterfaceType.Name} -- {meta.Export.ImplementType?.Name}" 
                : $"{meta.InterfaceType.Name}|{meta.ContractName} -- {meta.Export.ImplementType?.Name}";

        //([CanBeNull] Type type, [CanBeNull] object exportName)
        //{
        //    if (type == null) type = typeof(object);
        //    return "[" + type.Name + "|" + exportName + "]";
        public static string FormatExport(ImportMetadata meta) => $"{meta.InterfaceType?.Name}|{meta.ContractName} -- {meta.Export.ImplementType?.Name}";

        public static string FormatExport(Type type, string contractName, string msg) => $"{type?.Name}|{contractName} -- {msg}";
    }
}