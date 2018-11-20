using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public class ReaderWriterLockHolder : BaseHolder<ReaderWriterLockSlim>
    {
        public ReaderWriterLockHolder([CanBeNull] ReaderWriterLockSlim lockSlim)
            : base(lockSlim ?? new ReaderWriterLockSlim())
        {
        }

        public ReaderWriterLockHolder()
            : base(new ReaderWriterLockSlim())
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class ReaderWriterLockSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                new ReaderWriterLockHolder(
                    info
                        .GetInvokeMember
                            <ReaderWriterLockSlim>(target))
                {
                    Name = HolderName
                });
        }
    }

    [PublicAPI]
    public enum ReaderWriterLockBehavior
    {
        Invalid,
        Read,
        Write
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public sealed class ReaderWriterLockAttribute : ThreadingBaseAspect
    {
        public ReaderWriterLockBehavior LockBehavior { get; set; }

        private Func<bool> _enter;

        private Action _exit;

        private ReaderWriterLockHolder _holder;

        private Func<bool> _skip;

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                context,
                () =>
                    new ReaderWriterLockHolder(),
                HolderName ?? string.Empty);
            switch (LockBehavior)
            {
                case ReaderWriterLockBehavior.Read:
                    _enter = () => _holder.Value.TryEnterReadLock(-1);
                    _exit = _holder.Value.ExitReadLock;
                    _skip = () => _holder.Value.IsReadLockHeld;
                    break;
                case ReaderWriterLockBehavior.Write:
                    _enter = () => _holder.Value.TryEnterWriteLock(-1);
                    _exit = _holder.Value.ExitWriteLock;
                    _skip = () => _holder.Value.IsWriteLockHeld;
                    break;
                default:
                    CommonConstants.LogCommon(false, "AOP Module: Invalid Reader WriterLogBahavior: {0}", target);
                    break;
            }

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            if (_skip == null) return;

            var skipping = _skip();
            var taken = false;

            try
            {
                if (!skipping) taken = _enter();

                invocation.Proceed();
            }
            finally
            {
                if (taken) _exit();
            }
        }
    }
}