using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(
    AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
    | AttributeTargets.Property, AllowMultiple = false)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class LevelSpecificImport : InjectAttribute
    {
        public const string LevelMetadata = "UsedLevel";

        public int Level { get; private set; }

        public LevelSpecificImport(int level)
        {
            Level = level;
        }

        public override Dictionary<string, object> CreateMetadata()
        {
            return new Dictionary<string, object> { { LevelMetadata, Level } };
        }
    }
}