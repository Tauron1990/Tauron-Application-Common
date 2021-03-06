﻿using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc
{
    [Serializable]
    public abstract class BuildParameter
    {
        [CanBeNull]
        protected internal abstract IExport CreateExport();
    }
}