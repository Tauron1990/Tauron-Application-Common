using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;
using Tauron.Proxy.Core;

namespace Tauron.Proxy
{
    [PublicAPI]
    public class MixinData : SealableObject
    {
        private object _target;

        [NotNull]
        public object Target
        {
            get
            {
                Contract.Ensures(Contract.Result<Object>() != null);

                return _target;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Target");

                _target = value;
            }
        }

        [NotNull]
        public Type MixinType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);

                return Target.GetType();
            }
        }

        [NotNull]
        public IEnumerable<Type> Interfaces
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

                MixinType.GetInterfaces();
            }
        }



        public MixinData([NotNull] object target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");

            Target = target;
        }
    }
}