using System;
using System.Text;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    public sealed class ErrorTracer
    {
        private readonly StringBuilder _internalPhase = new StringBuilder();
        private int _ident;

        public bool Exceptional { get; set; }

        [NotNull]
        public string Phase
        {
            get
            {
                return _internalPhase.ToString();
            }
            set
            {
                for (int i = 0; i < _ident; i++)
                {
                    _internalPhase.Append('\t');
                }
                _internalPhase.AppendLine(value);
            }
        }

        public void IncrementIdent()
        {
            _ident++;
        }
        public void DecrementIdent()
        {
            _ident--;
        }

        [CanBeNull]
        public string Export { get; set; }

        [CanBeNull]
        public Exception Exception { get; set; }

        public static string FormatExport([CanBeNull]Type type, [CanBeNull]string exportName)
        {
            if (type == null)
                type = typeof (object);
            return "[" + type.Name + "|" + exportName + "]";
        }
    }
}