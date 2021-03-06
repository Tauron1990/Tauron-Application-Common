﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Models
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class InjectModelAttribute : InjectAttribute
    {
        public InjectModelAttribute([NotNull] string name)
            : base(typeof(IModel)) => ContractName = Argument.NotNull(name, nameof(name));

        public bool EnablePropertyInheritance { get; set; }

        public override Dictionary<string, object> CreateMetadata() => new Dictionary<string, object>
        {
            {PropertyModelExtension.EnablePropertyInheritanceMetadataName, EnablePropertyInheritance}
        };
    }
}