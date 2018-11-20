using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.LifeTime
{
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public abstract class LifetimeContextAttribute : ExportMetadataBaseAttribute
    {
        protected LifetimeContextAttribute([NotNull] Type lifeTimeType)
            : base(AopConstants.LiftimeMetadataName, null)
        {
            Argument.NotNull(lifeTimeType, nameof(lifeTimeType));

            LifeTimeType = lifeTimeType;
            ShareLiftime = true;
            InternalValue = this;
        }

        [NotNull]
        public Type LifeTimeType { get; }

        public bool ShareLiftime { get; set; }
    }
}