﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [Export(typeof(IRuleFactory))]
    [PublicAPI]
    public sealed class RuleFactory : IRuleFactory
    {
        private readonly IDictionary<string, IRuleBase> _cache = new Dictionary<string, IRuleBase>();

        [InjectRepositoryFactory]
        private RepositoryFactory _repositoryFactory;

        [Inject(typeof(IRuleBase))]
        private InstanceResolver<IRuleBase, IRuleMetadata>[] _rules;

        private IRuleBase GetOrCreate(string name)
        {
            if (_cache.TryGetValue(name, out var cRule)) return cRule;

            var rule = _rules.SingleOrDefault(i => i.Metadata.Name == name)?.Resolve();

            _cache[name] = rule ?? throw new InvalidOperationException("No Rule Found -- " + name);
            DatalayerHelper.InitializeRule(rule, _repositoryFactory);

            return rule;
        }

        public IRuleBase Create(string name) => GetOrCreate(name);

        public IBusinessRule CreateBusinessRule(string name) => (IBusinessRule) Create(name);

        public IIBusinessRule<TType> CreateIiBusinessRule<TType>(string name) => (IIBusinessRule<TType>) Create(name);

        public IIOBusinessRule<TInput, TOutput> CreateIioBusinessRule<TInput, TOutput>(string name)
            //where TOutput : class where TInput : class
            => (IIOBusinessRule<TInput, TOutput>) Create(name);

        public IOBussinesRule<TOutput> CreateOBussinesRule<TOutput>(string name)
            //where TOutput : class
            => (IOBussinesRule<TOutput>) Create(name);

        public CompositeRule<TInput, TOutput> CreateComposite<TInput, TOutput>(params string[] names)
        {
            var keyBuilder = new StringBuilder();

            foreach (var name in names) keyBuilder.Append(name);

            var key = keyBuilder.ToString();
            CompositeRule<TInput, TOutput> compositeule;

            if (_cache.TryGetValue(key, out var cache))
                compositeule = (CompositeRule<TInput, TOutput>) cache;
            else
            {
                compositeule = new CompositeRule<TInput, TOutput>(names.Select(GetOrCreate));
                _cache[key] = compositeule;
            }

            DatalayerHelper.InitializeRule(compositeule, _repositoryFactory);
            return compositeule;
        }

        public bool CheckReturnType(string name, Type returnType)
        {
            var rule = Create(name);
            if (rule is IRuleDescriptor descriptor)
                return returnType.IsAssignableFrom(descriptor.ReturnType);
            return true;
        }
    }
}