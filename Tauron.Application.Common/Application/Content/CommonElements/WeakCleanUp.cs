using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    [PublicAPI]
    public sealed class WeakDelegate : IWeakReference, IEquatable<WeakDelegate>
    {
        public bool IsAlive => _reference == null || _reference.IsAlive;
        
        private readonly MethodBase _method;
        
        private readonly WeakReference _reference;
        
        public WeakDelegate([NotNull] Delegate @delegate)
        {
            Argument.NotNull(@delegate, nameof(@delegate));

            _method = @delegate.Method;

            if (!_method.IsStatic) _reference = new WeakReference(@delegate.Target);
        }

        public WeakDelegate([NotNull] MethodBase methodInfo, [NotNull] object target)
        {
            _method = Argument.NotNull(methodInfo, nameof(methodInfo));
            _reference = new WeakReference(Argument.NotNull(target, nameof(target)));
        }
        
        public bool Equals(WeakDelegate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other._reference.Target == _reference.Target && other._method == _method;
        }
        
        public static bool operator ==(WeakDelegate left, WeakDelegate right)
        {
            var leftnull = ReferenceEquals(left, null);
            var rightNull = ReferenceEquals(right, null);

            return !leftnull ? left.Equals(right) : rightNull;
        }
        
        public static bool operator !=(WeakDelegate left, WeakDelegate right)
        {
            var leftnull = ReferenceEquals(left, null);
            var rightNull = ReferenceEquals(right, null);

            if (!leftnull) return !left.Equals(right);
            return !rightNull;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is WeakDelegate @delegate && Equals(@delegate);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                object target;
                return (((target = _reference.Target) != null ? target.GetHashCode() : 0) * 397)
                       ^ _method.GetHashCode();
            }
        }

        [CanBeNull]
        public object Invoke([CanBeNull] params object[] parms)
        {
            if (_method.IsStatic) return _method.Invoke(null, parms);

            var target = _reference.Target;
            return target == null ? null : _method.Invoke(target, parms);
        }
    }
    
    [PublicAPI]
    public static class WeakCleanUp
    {
        public const string WeakCleanUpExceptionPolicy = "WeakCleanUpExceptionPolicy";
        
        public static void RegisterAction([NotNull] Action action)
        {
            lock (Actions)
                Actions.Add(new WeakDelegate(Argument.NotNull(action, nameof(action))));
        }

        private static readonly List<WeakDelegate> Actions = Initialize();

        private static Timer _timer;

        private static readonly Logger Logger = LogManager.GetLogger(nameof(WeakCleanUp));
        
        private static List<WeakDelegate> Initialize()
        {
            _timer = new Timer(InvokeCleanUp, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return new List<WeakDelegate>();
        }
        
        private static void InvokeCleanUp(object state)
        {
            lock (Actions)
            {
                var dead = new List<WeakDelegate>();
                foreach (var weakDelegate in Actions.ToArray())
                    if (weakDelegate != null && weakDelegate.IsAlive)
                    {
                        try
                        {
                            weakDelegate.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                    else
                        dead.Add(weakDelegate);

                dead.ForEach(del => Actions.Remove(del));
            }
        }
    }
}