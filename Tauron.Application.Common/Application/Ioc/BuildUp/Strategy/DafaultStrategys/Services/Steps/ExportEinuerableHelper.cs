using System.Collections.Generic;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.SimpleWorkflow;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ExportEnumeratorHelper
    {
        private readonly IEnumerator<ExportMetadata> _metaEnumerator;
        private readonly ReflectionContext _context;
        private bool _ok;

        public ExportEnumeratorHelper([NotNull] IEnumerator<ExportMetadata> metaEnumerator, [NotNull] ReflectionContext context)
        {
            _metaEnumerator = metaEnumerator;
            _context = context;
        }

        public StepId NextId { get { return _ok ? StepId.None : StepId.LoopEnd; } }

        public void MoveNext()
        {
            _ok = _metaEnumerator.MoveNext();
            _context.ExportMetadataOverride = _ok ? _metaEnumerator.Current : null;
        }
    }
}