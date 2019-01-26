using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application
{
    [Serializable]
    [PublicAPI]
    [DebuggerStepThrough]
    public abstract class BaseObject : IContextHolder
    {
        private static Dictionary<string, object> _singletons;

        [NonSerialized]
        private ObjectContext _context;

        [CanBeNull]
        ObjectContext IContextHolder.Context
        {
            get => _context;
            set => _context = value;
        }

        protected T GetSingleton<T>(Func<T> factory, [CallerMemberName] string name = null)
            where T : class
        {
            if (name == null) return null;

            if (_singletons == null)
                _singletons = new Dictionary<string, object>();

            var result = _singletons.TryGetAndCast<T>(name);
            if (result != null) return result;

            var value = factory();
            _singletons[name] = factory;
            return value;
        }
    }
}