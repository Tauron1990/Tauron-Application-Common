using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.Components
{
    [PublicAPI, DebuggerStepThrough]
    public sealed class ExportRegistry
    {
        private readonly ExportList _registrations = new ExportList();
        
        private class ExportEntry : GroupDictionary<Type, IExport>
        {
            public ExportEntry()
                : base(true)
            {
            }
        }

        private class ExportList : SortedList<int, ExportEntry>
        {
            public ExportList()
                : base(new DescendingComparer())
            {
            }

            public void Add(int location, [NotNull] Type type, [NotNull] IExport export)
            {
                if (TryGetValue(location, out var entry))
                    entry[type].Add(export);
                else
                {
                    entry = new ExportEntry();
                    entry[type].Add(export);
                    Add(location, entry);
                }
            }

            [CanBeNull]
            public IEnumerable<ExportMetadata> Lookup([NotNull] Type type, [CanBeNull] string contractName, int at)
            {
                var realExports = new HashSet<ExportMetadata>();

                foreach (var pair in this.Where(p => p.Key <= at))
                    if (pair.Value.TryGetValue(type, out var exports))
                        exports.SelectMany(ep => ep.SelectContractName(contractName)).Foreach(ex => realExports.Add(ex));

                return realExports.Count == 0 ? null : realExports;
            }

            public void RemoveValue([NotNull] IExport export)
            {
                foreach (var value in Values.ToArray()) value.RemoveValue(export);
            }

            private class DescendingComparer : IComparer<int>
            {
                public int Compare(int x, int y)
                {
                    if (x < y)
                        return 1;
                    if (x > y)
                        return -1;
                    return 0;
                }
            }
        }
        
        [NotNull]
        public IEnumerable<ExportMetadata> FindAll([NotNull] Type type, [CanBeNull] string contractName, [NotNull] ErrorTracer errorTracer, int limit = int.MaxValue)
        {
            Argument.NotNull(type, nameof(type));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                lock (this)
                {
                    errorTracer.Phase = "Getting Exports by Type (" + type + ")";
                    var regs = _registrations.Lookup(type, contractName, limit);

                    if (regs == null)
                        return Enumerable.Empty<ExportMetadata>();

                    errorTracer.Phase = "Filtering Exports by Contract Name (" + contractName + ")";
                    return regs.Where(exp => exp != null);
                }
            }
            catch (Exception e)
            {
                errorTracer.Exception = e;
                errorTracer.Exceptional = true;
                errorTracer.Export = ErrorTracer.FormatExport(type, contractName, e.Message);

                return Enumerable.Empty<ExportMetadata>();
            }
        }

        [CanBeNull]
        public ExportMetadata FindOptional([NotNull] Type type, [CanBeNull] string contractName, [NotNull] ErrorTracer errorTracer, int level = int.MaxValue)
        {
            Argument.NotNull(type, nameof(type));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            lock (this)
            {
                var arr = FindAll(type, contractName, errorTracer).ToArray();

                if (errorTracer.Exceptional) return null;

                errorTracer.Phase = "Getting Single Instance";

                if (arr.Length <= 1) return arr.Length == 0 ? null : arr[0];

                errorTracer.Exceptional = true;
                errorTracer.Export = ErrorTracer.FormatExport(type, contractName, "More then One Export Found");

                return arr.Length == 0 ? null : arr[0];
            }
        }


        [CanBeNull]
        public ExportMetadata FindSingle([NotNull] Type type, [NotNull] string contractName, [NotNull] ErrorTracer errorTracer, int level = int.MaxValue)
        {
            Argument.NotNull(type, nameof(type));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            var temp = FindOptional(type, contractName, errorTracer, level);
            if (errorTracer.Exceptional) return null;
            if (temp != null) return temp;

            errorTracer.Exceptional = true;
            errorTracer.Export = ErrorTracer.FormatExport(type, contractName, "No Export Found");

            return null;
        }

        public void Register([NotNull] IExport export, int level)
        {
            Argument.NotNull(export, nameof(export));
            lock (this)
            {
                foreach (var type in export.Exports)
                    _registrations.Add(level, type, export);
            }
        }

        public void Remove([NotNull] IExport export)
        {
            Argument.NotNull(export, nameof(export));
            lock (this)
                _registrations.RemoveValue(export);
        }

        [CanBeNull]
        public IExport TryFindExport(object toBuild)
        {
            Type toBuildType = toBuild.GetType();

            lock (this)
                return _registrations.Values.SelectMany(e => e.AllValues).FirstOrDefault(export => export.ImplementType.IsAssignableFrom(toBuildType));
        }
    }
}