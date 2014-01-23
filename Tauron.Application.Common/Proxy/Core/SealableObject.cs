using System;

namespace Tauron.Proxy.Core
{
    public class SealableObject
    {
        private bool _isSealed;

        public void Seal()
        {
            _isSealed = true;
        }

        protected void CheckSeal()
        {
            if (_isSealed)
                throw new InvalidOperationException("Setting are Sealed");
        }
    }
}