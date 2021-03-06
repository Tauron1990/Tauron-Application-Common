﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class LevelSpecificImport : InjectAttribute
    {
        public const string LevelMetadata = "UsedLevel";

        public LevelSpecificImport(int level) => Level = level;

        public int Level { get; private set; }

        public override Dictionary<string, object> CreateMetadata() => new Dictionary<string, object> {{LevelMetadata, Level}};
    }
}