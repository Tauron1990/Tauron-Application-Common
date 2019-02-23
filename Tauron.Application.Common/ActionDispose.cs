using System;

namespace Tauron
{
    public sealed class ActionDispose : IDisposable
    {
        private readonly Action _action;

        public ActionDispose(Action action) => _action = action;

        public void Dispose() => _action();
    }
}