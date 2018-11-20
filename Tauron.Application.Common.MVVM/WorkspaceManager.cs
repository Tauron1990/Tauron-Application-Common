using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public sealed class WorkspaceManager<TWorkspace> : UIObservableCollection<TWorkspace>
        where TWorkspace : class, ITabWorkspace
    {
        public WorkspaceManager([NotNull] IWorkspaceHolder holder) => _holder = Argument.NotNull(holder, nameof(holder));
        
        [NotNull]
        public ITabWorkspace ActiveItem

        {
            get => _activeItem;
            set
            {
                _activeItem = value;

                if (Equals(_activeItem, value)) return;

                _activeItem?.OnDeactivate();

                _activeItem = value;
                _activeItem.OnActivate();

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ActiveItem)));
            }
        }

        public new void AddRange([NotNull] IEnumerable<TWorkspace> items)
        {
            foreach (var item in Argument.NotNull(items, nameof(items)).Where(it => it != null))
                Add(item);
        }
        
        private void UnRegisterWorkspace([NotNull] ITabWorkspace space)
        {
            space.OnClose();
            _holder.UnRegister(space);
        }

        private readonly IWorkspaceHolder _holder;
        private ITabWorkspace _activeItem;
        
        protected override void ClearItems()
        {
            foreach (var workspace in Items) UnRegisterWorkspace(workspace);

            base.ClearItems();
        }
        
        protected override void InsertItem(int index, [CanBeNull] TWorkspace item)
        {
            if (item == null) return;

            if (index < Count) UnRegisterWorkspace(this[index]);

            _holder.Register(item);

            base.InsertItem(index, item);
        }
        
        protected override void RemoveItem(int index)
        {
            UnRegisterWorkspace(this[index]);
            base.RemoveItem(index);
        }
        
        protected override void SetItem(int index, [CanBeNull] TWorkspace item)
        {
            if (item == null) return;

            UnRegisterWorkspace(this[index]);
            _holder.Register(item);
            base.SetItem(index, item);
        }
    }
}