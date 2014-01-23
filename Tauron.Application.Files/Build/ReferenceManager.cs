using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Construction;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public abstract class ItemEntryBase
    {
        protected bool Equals([CanBeNull] ItemEntryBase other)
        {
            if (other == null) return false;
            return string.Equals(_include, other._include) && string.Equals(_itemType, other._itemType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return ((Include.GetHashCode())*397) ^ (_itemType != null ? _itemType.GetHashCode() : 0);
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }

        [PublicAPI]
        protected abstract class InternalPropertyBase
        {
            public abstract void BeginChangeItem();
            public abstract void EndChangeItem();
            public abstract void SetItem();
        }

        protected class InternalProperty<TType> : InternalPropertyBase
        {
            private readonly InternalPropertyAccessor<TType> _fieldAccessor;
            private readonly InternalPropertyAccessor<TType> _propertyAccessor;

            public InternalProperty([NotNull] InternalPropertyAccessor<TType> fieldAccessor, [NotNull] InternalPropertyAccessor<TType> propertyAccessor) 
            {
                _fieldAccessor = fieldAccessor;
                _propertyAccessor = propertyAccessor;
            }

            public override void BeginChangeItem()
            {
                _fieldAccessor.Setter(_propertyAccessor.Getter());
            }

            public override void EndChangeItem()
            {
                _propertyAccessor.Setter(_fieldAccessor.Getter());
            }

            public override void SetItem()
            {
                _propertyAccessor.Setter(_fieldAccessor.Getter());
            }
        }

        private string _include;
        private string _itemType;
        private Dictionary<string, InternalPropertyBase> _propertys = new Dictionary<string, InternalPropertyBase>();

        protected ItemEntryBase()
        {
            RegisterProperty("InternalInclude",
                new InternalProperty<string>(
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => _include = b, () => _include),
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => Include = b, () => Include)));

            RegisterProperty("InternalItemType",
                new InternalProperty<string>(
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => _itemType = b, () => _itemType),
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => ItemType = b, () => ItemType)));
        }

        [CanBeNull]
        protected ProjectItemElement RootElement { get; set; }

        [NotNull]
        public string ItemType
        {
            get
            {
                return RootElement != null ? RootElement.ItemType : _itemType;
            }
            protected set
            {
                if (RootElement != null)
                    RootElement.ItemType = value;

                _itemType = value;
            }
        }

        [NotNull]
        public string Include
        {
            get
            {
                if(string.IsNullOrEmpty(_include))
                    return RootElement != null ? RootElement.Include : string.Empty;
                
                return _include;
            }
            set
            {
                if (RootElement != null)
                    RootElement.Include = value;
                _include = value;
            }
        }

        internal virtual void Init([CanBeNull] ProjectItemElement rootElement)
        {
            RootElement = rootElement;
        }

        internal void ChangeItem([NotNull] ProjectItemElement element)
        {
            foreach (var value in _propertys.Values)
            {
                value.BeginChangeItem();
            }

            RootElement = element;

            foreach (var value in _propertys.Values)
            {
                value.EndChangeItem();
            }
        }

        internal virtual void SetItem([NotNull] ProjectItemElement element)
        {
            RootElement = element;

            foreach (var value in _propertys.Values)
            {
                value.SetItem();
            }
        }

        protected void RegisterProperty([NotNull] string name, [NotNull] InternalPropertyBase property)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (property == null) throw new ArgumentNullException("property");

            _propertys[name] = property;
        }
        protected void RemoveProperty([NotNull] string name)
        {
            _propertys.Remove(name);
        }

        [CanBeNull]
        protected string GetMetatdataValue([NotNull]string name)
        {
            if (RootElement == null)
                return null;
            var element = RootElement.Metadata.FirstOrDefault(meta => meta.Name == name);
            
            return element != null ? element.Value : null;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren", MessageId = "1", Justification = "Reviewed")]
        protected void SetMetadataValue([NotNull] string name, [CanBeNull] object value)
        {
            if(RootElement == null) return;

            var element = RootElement.Metadata.FirstOrDefault(meta => meta.Name == name);
            if(element == null && value == null) return;

            if (element != null && value == null)
            {
                RootElement.RemoveChild(element);
                return;
            }

            if (element != null)
            {
                element.Value = value.ToString();
                return;
            }

            RootElement.AddMetadata(name, value.ToString());
        }

        public override bool Equals([CanBeNull] object obj)
        {
            var entry = obj as ItemEntryBase;
            if (entry == null) return false;

            return entry.Include == Include && entry.ItemType == ItemType;
        }

        protected class InternalPropertyAccessor<TType>
        {
            [NotNull]
            public Action<TType> Setter { get; private set; }

            [NotNull]
            public Func<TType> Getter { get; private set; }

            public InternalPropertyAccessor([NotNull] Action<TType> setter, [NotNull] Func<TType> getter)
            {
                Setter = setter;
                Getter = getter;
            }
        }
    }

    [PublicAPI]
    public class ReferenceEntry : ItemEntryBase
    {
        public const string SpecificVersionProperty = "SpecificVersion";
        public const string HintPathProperty = "HintPath";
        public const string RequiredTargetFrameworkPropery = "RequiredTargetFramework";

        private bool? _specificVersion;
        private string _hintPath;
        private string _requiredTargetFramework;

        public ReferenceEntry()
        {
            ItemType = "Reference";

            RegisterProperty(SpecificVersionProperty,
                new InternalProperty<bool?>(
                    new ItemEntryBase.InternalPropertyAccessor<bool?>(b => _specificVersion = b, () => _specificVersion),
                    new ItemEntryBase.InternalPropertyAccessor<bool?>(b => SpecificVersion = b, () => SpecificVersion)));

            RegisterProperty(HintPathProperty,
                new InternalProperty<string>(
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => _hintPath = b, () => _hintPath),
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => HintPath = b, () => HintPath)));

            RegisterProperty(RequiredTargetFrameworkPropery,
                new InternalProperty<string>(
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => _requiredTargetFramework = b,
                        () => _requiredTargetFramework),
                    new ItemEntryBase.InternalPropertyAccessor<string>(b => RequiredTargetFramework = b,
                        () => RequiredTargetFramework)));
        }

        [CanBeNull]
        public bool? SpecificVersion
        {
            get
            {
                string value = GetMetatdataValue(SpecificVersionProperty);
                
                return value == null ? _specificVersion : bool.Parse(value);
            }
            set
            {
                SetMetadataValue(SpecificVersionProperty, value);
                _specificVersion = value;
            }
        }

        [CanBeNull]
        public string HintPath
        {
            get { return GetMetatdataValue(HintPathProperty) ?? _hintPath; }
            set
            {
                SetMetadataValue(HintPathProperty, value);
                _hintPath = value;
            }
        }

        [CanBeNull]
        public string RequiredTargetFramework
        {
            get { return GetMetatdataValue(RequiredTargetFrameworkPropery) ?? _requiredTargetFramework; }
            set
            {
                SetMetadataValue(RequiredTargetFrameworkPropery, value);
                _requiredTargetFramework = value;
            }
        }
    }

    [PublicAPI]
    public class ProjectReferenceItem : ItemEntryBase
    {
        public const string ProjectPropery = "Project";
        public const string NameProperty = "Name";

        private string _name;
        private Guid _projectGuid;

        public ProjectReferenceItem()
        {
            ItemType = "ProjectItem";
        
            
        }

        [CanBeNull]
        public string Name
        {
            get
            {
                string value = GetMetatdataValue(NameProperty);

                return value ?? _name;
            }
            set
            {
                SetMetadataValue(NameProperty, value);
                _name = value;
            }
        }

        public Guid Project
        {
            get
            {
                string value = GetMetatdataValue(NameProperty);

                return value == null ? _projectGuid : Guid.Parse(value);
            }
            set
            {
                SetMetadataValue(NameProperty, value);
                _projectGuid = value;
            }
        }
    }

    [PublicAPI]
    public class ValueEventArgs : EventArgs
    {
        [NotNull]
        public ItemEntryBase Value { get; private set; }

        public ValueEventArgs([NotNull] ItemEntryBase value)
        {
            Value = value;
        }
    }

    [PublicAPI]
    public class ItemSwitchArgs : ValueEventArgs
    {
        [NotNull]
        public ItemEntryBase Switched { get; private set; }

        public ItemSwitchArgs([NotNull] ItemEntryBase value, [NotNull] ItemEntryBase switched) : base(value)
        {
            Switched = switched;
        }
    }

    [PublicAPI]
    public class ItemListCleared : EventArgs
    {
        [NotNull]
        public IEnumerable<ItemEntryBase> OldValues { get; set; }

        public ItemListCleared([NotNull] IEnumerable<ItemEntryBase> oldValues)
        {
            OldValues = oldValues;
        }
    }

    [PublicAPI]
    public abstract class ItemBaseList<TValue> : IList<TValue>
        where TValue : ItemEntryBase
    {
        public event EventHandler<ItemListCleared>  Cleared;

        protected void OnCleared([NotNull] ItemListCleared e)
        {
            EventHandler<ItemListCleared> handler = Cleared;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ValueEventArgs> Removed;

        public event EventHandler<ItemSwitchArgs> ItemSwitched;

        protected void OnItemSwitched([NotNull] ItemSwitchArgs e)
        {
            EventHandler<ItemSwitchArgs> handler = ItemSwitched;
            if (handler != null) handler(this, e);
        }

        protected void OnRemoved([NotNull] ValueEventArgs e)
        {
            var handler = Removed;
            if (handler != null)
                handler(this, e);
        }

        private ProjectItemGroupElement _baseElement;
        
        private readonly List<TValue> _cache = new List<TValue>();
        private List<TValue> _externalCache = new List<TValue>();

        //protected ItemBaseList([NotNull] ProjectItemGroupElement baseElement)
        //{
        //    _baseElement = baseElement;

        //    InitializeCache();
        //}

        //protected ItemBaseList()
        //{
        //}

        protected void InitializeCache([NotNull] ProjectItemGroupElement baseElement)
        {
            if (baseElement == null) throw new ArgumentNullException("baseElement");

            _baseElement = baseElement;

            InitializeCache();
        }

        private void InitializeCache()
        {
            foreach (var item in _baseElement.Items.Where(Filter))
            {
                _cache.Add(CreateValue(item));
            }
        }

        [NotNull]
        private ProjectItemElement AddSimple([NotNull] TValue item)
        {
            return AddItem(_baseElement.AddItem, item);
        }

        protected virtual bool Filter([NotNull] ProjectItemElement element)
        {
            return true;
        }

        [NotNull]
        protected abstract TValue CreateValue([NotNull] ProjectItemElement element);
        [NotNull]
        protected abstract ProjectItemElement AddItem([NotNull] Func<string, string, ProjectItemElement> creatorFunc, [NotNull] TValue item);
        [NotNull]
        protected abstract string GetInclude([NotNull] TValue item);
        protected abstract void ChangeItem([NotNull] TValue newItem, [NotNull] ProjectItemElement element);

        public IEnumerator<TValue> GetEnumerator()
        {
            return _cache.Concat(_externalCache).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AddSimple(item);
            _cache.Add(item);
        }

        public void Add([NotNull] TValue item, bool external)
        {
            if (item == null) throw new ArgumentNullException("item");
            if(external) _externalCache.Add(item);
            else Add(item);
        }

        public void Clear()
        {
            _baseElement.RemoveAllChildren();
            _cache.Clear();
            var old = _externalCache.ToArray();
            _externalCache.Clear();
            OnCleared(new ItemListCleared(old));
        }

        public bool Contains([NotNull] TValue item)
        {
            return _cache.Contains(item) || _externalCache.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _cache.CopyTo(array, arrayIndex);
            arrayIndex += _cache.Count;
            _externalCache.CopyTo(array, arrayIndex);
        }

        public bool Remove([NotNull] TValue item)
        {
            if (_externalCache.Remove(item))
            {
                OnRemoved(new ValueEventArgs(item));
                return true;
            }

            string name = GetInclude(item);
            var itm = _baseElement.Items.FirstOrDefault(innerItem => innerItem.Include == name);
            if (itm == null)
                return false;
            _baseElement.RemoveChild(itm);
            return _cache.Remove(item);
        }

        public int Count { get { return _cache.Count + _externalCache.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf([NotNull] TValue item)
        {
            int index = _cache.IndexOf(item);
            if (index != -1) return index;

            index = _externalCache.IndexOf(item);
            if (index == -1) return -1;

            return _cache.Count + index;
        }

        public void Insert(int index, [NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (index >= _cache.Count)
            {
                Add(item);
                return;
            }

            string name = GetInclude(item);
            var oldItem = _baseElement.Items.First(itm => itm.Include == name);
            _baseElement.InsertAfterChild(AddSimple(item), oldItem);
        }

        public void RemoveAt(int index)
        {
            _cache.RemoveAt(index);
            _baseElement.RemoveChild(_baseElement.Items.ElementAt(index));
        }

        public TValue this[int index]
        {
            get
            {
                if (_cache.Count == index)
                    return _externalCache[0];
                
                return index > _cache.Count ? _externalCache[index - _cache.Count] : _cache[index];
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                if (index >= _cache.Count)
                {
                    index = index - _cache.Count;

                    var extOld = _externalCache[index];

                    _externalCache[index] = value;

                    OnItemSwitched(new ItemSwitchArgs(value, extOld));
                    return;
                }

                var oldName = GetInclude(_cache[index]);
                var old = _baseElement.Items.First(itm => itm.Include == oldName);

                ChangeItem(value, old);

                _cache[index] = value;
            }
        }
    }
    public class ItemEntryBaseList<TType> : ItemBaseList<TType>
        where TType : ItemEntryBase, new()
    {
        private readonly string _itemFilter;
        private readonly string[] _nonItemFilter;

        public ItemEntryBaseList([NotNull] ProjectItemGroupElement baseElement)
            : this(baseElement, null, null)
        {
        }

        public ItemEntryBaseList([NotNull] ProjectItemGroupElement baseElement, [CanBeNull] string itemFilter,
            [CanBeNull] params string[] nonItemFilter)
        {
            _itemFilter = itemFilter;
            _nonItemFilter = nonItemFilter;

            InitializeCache(baseElement);
        }

        protected override bool Filter(ProjectItemElement element)
        {
            if (element == null) return false;

            if (_itemFilter == null && _nonItemFilter == null)
                return base.Filter(element);

            bool ok = _itemFilter == null || element.ItemType == _itemFilter;
            if (_nonItemFilter != null && ok)
                ok = !_nonItemFilter.Contains(element.ItemType);

            return ok;
        }

        protected override TType CreateValue(ProjectItemElement element)
        {
            var temp = new TType();
            temp.Init(element);
            return temp;
        }

        protected override ProjectItemElement AddItem(Func<string, string, ProjectItemElement> creatorFunc, TType item)
        {
            if (creatorFunc == null) return null;

            var element = creatorFunc(item.ItemType, item.Include);
            item.SetItem(element);
            return element;
        }

        protected override string GetInclude( TType item)
        {
            return item.Include;
        }

        protected override void ChangeItem(TType newItem, ProjectItemElement element)
        {
            newItem.ChangeItem(element);
        }
    }

    [PublicAPI]
    public sealed class ReferenceManager
    {
        private class ReferenceEntryList : ItemEntryBaseList<ReferenceEntry>
        {
            public ReferenceEntryList([NotNull] ProjectItemGroupElement baseElement) : base(baseElement)
            {
            }
        }
        private class ProjectReferenceList : ItemEntryBaseList<ProjectReferenceItem>
        {
            public ProjectReferenceList([NotNull] ProjectItemGroupElement baseElement) : base(baseElement)
            {
            }
        }

        [NotNull]
        public IList<ProjectReferenceItem> ProjectReferences { get; private set; }

        [NotNull]
        public IList<ReferenceEntry> ReferenceEntries { get; private set; }

        [NotNull]
        internal ProjectItemGroupElement[] UsedItemGroupElements { get; private set; }

        internal ReferenceManager([NotNull] ProjectRootElement root)
        {
            UsedItemGroupElements = new ProjectItemGroupElement[2];

            foreach (var itemGroup in root.ItemGroups.Where(itemGroup => itemGroup.Count != 0))
            {
                if(itemGroup.Items.All(it => it.ItemType == "Reference"))
                {
                    ReferenceEntries = new ReferenceEntryList(itemGroup);
                    UsedItemGroupElements[0] = itemGroup;
                }
                if (itemGroup.Items.All(it => it.ItemType != "ProjectReference")) continue;
                
                ProjectReferences = new ProjectReferenceList(itemGroup);
                UsedItemGroupElements[1] = itemGroup;
            }

            if (ProjectReferences == null)
            {
                var pRefs = root.AddItemGroup();
                ProjectReferences = new ProjectReferenceList(pRefs);
            }

            if (ReferenceEntries != null) return;

            var rRefs = root.AddItemGroup();
            ReferenceEntries = new ReferenceEntryList(rRefs);
        }
    }
}
