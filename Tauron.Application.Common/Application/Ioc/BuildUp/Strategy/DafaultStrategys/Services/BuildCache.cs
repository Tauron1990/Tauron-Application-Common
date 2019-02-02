﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public sealed class BuildCache : ICache, IDisposable
    {
        public BuildCache()
        {
            WeakCleanUp.RegisterAction(OnCleanUp);
        }

        private void OnCleanUp()
        {
            lock (this)
            {
                IEnumerable<IExport> deadKeysOne =
                    _global.Where(ent => !ent.Value.IsAlive).Select(ent => ent.Key).ToArray();
                IEnumerable<ExportMetadata> deadkeysTwo =
                    (from ent in _local where !ent.Value.IsAlive select ent.Key).ToArray();

                foreach (var export in deadKeysOne) _global.Remove(export);

                foreach (var exportMetadata in deadkeysTwo) _local.Remove(exportMetadata);
            }
        }

        private readonly Dictionary<IExport, ILifetimeContext> _global = new Dictionary<IExport, ILifetimeContext>();

        private readonly Dictionary<ExportMetadata, ILifetimeContext> _local = new Dictionary<ExportMetadata, ILifetimeContext>();

        public void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime)
        {
            if(context == null)
                return;

            lock (this)
            {
                if (shareLifetime) _global[metadata.Export] = context;
                else _local[metadata] = context;
            }
        }

        public ILifetimeContext GetContext(ExportMetadata metadata)
        {
            lock (this)
            {
                ILifetimeContext context;
                if (metadata.Export.ShareLifetime) _global.TryGetValue(metadata.Export, out context);
                else _local.TryGetValue(metadata, out context);

                return context ?? NotSharedLifetime.DefaultNotSharedLifetime;
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                var toDispose = new List<IDisposable>();
                toDispose.AddRange(_global.Where(p => !p.Key.ExternalInfo.HandlesDispose).Select(p => p.Value?.GetValue()).OfType<IDisposable>());
                toDispose.AddRange(_local.Where(p => !p.Key.Export.ExternalInfo.HandlesDispose).Select(p => p.Value?.GetValue()).OfType<IDisposable>());

                foreach (var disposable in toDispose)
                    disposable.Dispose();

                _global.Clear();
                _local.Clear();
            }
        }
    }
}