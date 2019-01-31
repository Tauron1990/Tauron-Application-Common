using System;
using System.Collections.Generic;

namespace Tauron.Application
{
    public abstract class ComponentMessagnerBase : IComponentMessager
    {
        private class ActionRemover : IDisposable
        {
            private readonly ComponentMessagnerBase _container;
            private readonly Action<ComponentUpdate> _del;

            public ActionRemover(ComponentMessagnerBase container, Action<ComponentUpdate> del)
            {
                _container = container;
                _del = del;
            }

            public void Dispose()
            {
                _container._list.Remove(_del);
                if (_container._list.Count == 0)
                    _container._list = null;
            }
        }

        private List<Action<ComponentUpdate>> _list;

        protected void Publish(string component)
        {
            if(_list == null) return;

            foreach (var action in _list)
            {
                try
                {
                    action(new ComponentUpdate(component));
                }
                catch
                {
                    // ignored
                }
            }
        }

        IDisposable IComponentMessager.Subscribe(Action<ComponentUpdate> updateAction)
        {
            if (_list == null)
                _list = new List<Action<ComponentUpdate>>();
            _list.Add(updateAction);

            return new ActionRemover(this, updateAction);
        }
    }
}