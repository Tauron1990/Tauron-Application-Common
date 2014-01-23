using Tauron.JetBrains.Annotations;
using Tauron.Proxy.Core;

namespace Tauron.Proxy
{
    [PublicAPI]
    public class MixinConfiguration : SealableObject
    {
        private bool _allowSealedTypes;

        public bool AllowSealedTypes
        {
            get
            {
                return _allowSealedTypes;
            }
            set
            {
                CheckSeal();
                _allowSealedTypes = value;
            }
        }
    }
}