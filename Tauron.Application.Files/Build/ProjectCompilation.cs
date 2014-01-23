using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Construction;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public static class KnownTasks
    {
        public const string Compile = "Compile";
        public const string None = "None";
        public const string Page = "Page";
        public const string EmbeddedResource = "EmbeddedResource";
        public const string Resource = "Resource";
    }

    [PublicAPI]
    public enum CopyMode
    {
        None,
        Always,
        PreserveNewest
    }

    public abstract class KnownTaskBase : ItemEntryBase
    {
        public const string CopyToOutputDirectoryProperty = "CopyToOutputDirectory";
        public const string DependentUponProperty = "DependentUpon";
        public const string GeneratorProperty = "Generator";

        private CopyMode _copyToOutpuDirectory;
        private string _generator;

        protected KnownTaskBase([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            ItemType = name;

            RegisterProperty(CopyToOutputDirectoryProperty,
                new InternalProperty<CopyMode>(
                    new ItemEntryBase.InternalPropertyAccessor<CopyMode>(obj => _copyToOutpuDirectory = obj,
                        () => _copyToOutpuDirectory),
                    new ItemEntryBase.InternalPropertyAccessor<CopyMode>(obj => CopyToOutpuDirectory = obj,
                        () => CopyToOutpuDirectory)));

            RegisterProperty(GeneratorProperty,
                new InternalProperty<string>(
                    new ItemEntryBase.InternalPropertyAccessor<string>(obj => _generator = obj,
                        () => _generator),
                    new ItemEntryBase.InternalPropertyAccessor<string>(obj => Generator = obj,
                        () => Generator)));
        }

        public CopyMode CopyToOutpuDirectory
        {
            get
            {
                var temp = GetMetatdataValue(CopyToOutputDirectoryProperty);

                if (temp == null) return CopyMode.None;
                CopyMode mode;
                return Enum.TryParse(temp, true, out mode) ? mode : CopyMode.None;
            }
            set
            {
                _copyToOutpuDirectory = value;
                SetMetadataValue(CopyToOutputDirectoryProperty, value == CopyMode.None ? null : value.ToString());
            }
        }

        [CanBeNull]
        public string Generator
        {
            get
            {
                return GetMetatdataValue(GeneratorProperty) ?? _generator;
            }
            set
            {
                _generator = value;
                SetMetadataValue(GeneratorProperty, value);
            }
        }
    }

    [PublicAPI]
    public class CustomTask : ItemEntryBase
    {
        private class CustomProperty
        {
            private string _cache;
            private readonly string _name;
            private readonly Action<string, object> _setMetadata;

            public CustomProperty([NotNull] string name, [NotNull] Action<string, object> setMetadata)
            {
                _name = name;
                _setMetadata = setMetadata;
            }

            [CanBeNull]
            public string CacheValue
            {
                get { return _cache; }
            }

            public void SetValue([NotNull] string value)
            {
                _setMetadata(_name, value);
                _cache = value;
            }
        }

        private Dictionary<string, CustomProperty> _propertys = new Dictionary<string,CustomProperty>();
        private bool _skipMetadataSetting;

        public CustomTask([NotNull] string taskName, [NotNull] string include)
        {
            if (taskName == null) throw new ArgumentNullException("taskName");
            if (include == null) throw new ArgumentNullException("include");

            ItemType = taskName;
            Include = include;
        }

        public CustomTask()
        {
            ItemType = KnownTasks.None;
        }

        public void ChangeItemType([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            ItemType = name;
        }

        internal override void Init(ProjectItemElement rootElement)
        {
            base.Init(rootElement);

            ReInitializeCache();
        }

        internal override void SetItem(ProjectItemElement element)
        {
            base.SetItem(element);

            ReInitializeCache();
        }

        private void ReInitializeCache()
        {
            if(RootElement == null)
                return;
            try
            {
                _skipMetadataSetting = true;
                _propertys.Clear();

                foreach (var projectMetadataElement in RootElement.Metadata)
                {
                    SetCustomProperty(projectMetadataElement.Name, projectMetadataElement.Value);
                }
            }
            finally
            {
                _skipMetadataSetting = false;
            }
        }

        [CanBeNull]
        public string GetMetadata([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            string value = GetMetatdataValue(name);
            if (value != null) return value;

            CustomProperty prop;
            return _propertys.TryGetValue(name, out prop) ? prop.CacheValue : null;
        }

        public void SetMetadata([NotNull] string name, [CanBeNull] string value)
        {
            if (value == null)
            {
                SetMetadataValue(name, null);

                if (!_propertys.Remove(name)) return;

                RemoveProperty(name);
            }
            else
            {
                SetCustomProperty(name, value);
            }
        }

        private void SetCustomProperty([NotNull] string name, [NotNull] string value)
        {
            CustomProperty prop;
            if (_propertys.TryGetValue(name, out prop))
            {
                prop.SetValue(value);
            }
            else
            {
                prop = new CustomProperty(name, SetMetadataValueInternal);
                prop.SetValue(value);

                _propertys[name] = prop;
            }
        }

        private void SetMetadataValueInternal([NotNull] string name, [CanBeNull] object value)
        {
            if (_skipMetadataSetting)
                return;

            SetMetadataValue(name, value);
        }
    }

    [PublicAPI]
    public sealed class CompileTask : KnownTaskBase
    {
        public const string AutoGenProperty = "AutoGen";

        private bool? _autoGen;
        private string _dependentUpon;

        public CompileTask()
            : base(KnownTasks.Compile)
        {
            RegisterProperty(AutoGenProperty,
                             new InternalProperty<bool?>(
                                 new ItemEntryBase.InternalPropertyAccessor<bool?>(b => _autoGen = b, () => _autoGen),
                                 new ItemEntryBase.InternalPropertyAccessor<bool?>(b => AutoGen = b, () => AutoGen)));

            RegisterProperty(DependentUponProperty,
                             new InternalProperty<string>(
                                 new ItemEntryBase.InternalPropertyAccessor<string>(b => _dependentUpon = b, () => _dependentUpon),
                                 new ItemEntryBase.InternalPropertyAccessor<string>(b => DependentUpon = b, () => DependentUpon)));
        }

        public bool? AutoGen
        {
            get
            {
                string value = GetMetatdataValue(AutoGenProperty);

                return value != null ? Boolean.Parse(value) : _autoGen;
            }
            set
            {
                _autoGen = value;
                SetMetadataValue(AutoGenProperty, value);
            }
        }

        [NotNull]
        public string DependentUpon
        {
            get { return GetMetatdataValue(DependentUponProperty) ?? _dependentUpon; }
            set { _dependentUpon = value; SetMetadataValue(DependentUponProperty, value);}
        }
    }

    [PublicAPI]
    public sealed class PageTask : KnownTaskBase
    {
        public PageTask()
            : base(KnownTasks.Page)
        {
            Generator = "MSBuild:Compile";
        }
    }

    [PublicAPI]
    public sealed class EmbeddedResourceTask : KnownTaskBase
    {
        public EmbeddedResourceTask()
            : base(KnownTasks.EmbeddedResource)
        {
            
        }   
    }

    [PublicAPI]
    public sealed class ResourceTask : KnownTaskBase
    {
        public ResourceTask()
            : base(KnownTasks.Resource)
        {
            Generator = "MSBuild:Compile";
        }
    }

    [PublicAPI]
    public sealed class NoneTask : KnownTaskBase
    {
        public NoneTask()
            : base(KnownTasks.None)
        {
            
        }
    }

    [PublicAPI]
    public sealed class ProjectCompilation
    {
        private class CacheEntry
        {
            public CacheEntry([NotNull] ProjectItemElement element)
            {
                Element = element;
            }

            [NotNull]
            public ItemEntryBase Entry { get; set; }

            [NotNull]
            public ProjectItemElement Element { get; private set; }
        }

        private ProjectItemGroupElement _mainElement;
        private List<CacheEntry> _externalCache = new List<CacheEntry>();
        
        [NotNull]
        public IList<CustomTask> CustomTasks { get; private set; }

        [NotNull]
        public IList<CompileTask> CompileTasks { get; private set; }

        [NotNull]
        public IList<PageTask> PageTasks { get; private set; }

        [NotNull]
        public IList<EmbeddedResourceTask> EmbeddedResourceTasks { get; private set; }

        [NotNull]
        public IList<ResourceTask> ResourceTasks { get; private set; }

        [NotNull]
        public IList<NoneTask> NoneTasks { get; private set; }

        internal ProjectCompilation([NotNull] ProjectRootElement root,
            [NotNull] IEnumerable<ProjectItemGroupElement> usedItemGroupElements)
        {
            ProjectItemGroupElement[] allElements =
                root.ItemGroups.Where(ig => !usedItemGroupElements.Contains(ig)).ToArray();

            if (allElements.Length == 0)
                allElements = new[] {root.CreateItemGroupElement()};

            _mainElement = allElements[0];

            CustomTasks = CreateList<CustomTask>(null, KnownTasks.Compile,
                KnownTasks.EmbeddedResource, KnownTasks.None, KnownTasks.Page, KnownTasks.Resource);

            CompileTasks = CreateList<CompileTask>(KnownTasks.Compile, null);

            PageTasks = CreateList<PageTask>(KnownTasks.Page, null);

            EmbeddedResourceTasks = CreateList<EmbeddedResourceTask>(KnownTasks.EmbeddedResource, null);

            ResourceTasks = CreateList<ResourceTask>(KnownTasks.Resource, null);

            NoneTasks = CreateList<NoneTask>(KnownTasks.None, null);

            foreach (var item in allElements.Skip(1).SelectMany(ig => ig.Items))
            {
                switch (item.ItemType)
                {
                    case KnownTasks.Compile:
                        AddToList(CompileTasks, item);
                        break;
                    case KnownTasks.EmbeddedResource:
                        AddToList(EmbeddedResourceTasks, item);
                        break;
                    case KnownTasks.None:
                        AddToList(NoneTasks, item);
                        break;
                    case KnownTasks.Page:
                        AddToList(PageTasks, item);
                        break;
                    case KnownTasks.Resource:
                        AddToList(ResourceTasks, item);
                        break;
                    default:
                        AddToList(CustomTasks, item);
                        break;

                }
            }
        }

        private void AddToList<TType>([NotNull] IList<TType> list, [NotNull] ProjectItemElement element)
            where TType : ItemEntryBase, new()
        {
            var realList = (ItemBaseList<TType>) list;

            var temp = new TType();
            temp.Init(element);

            realList.Add(temp, true);
            _externalCache.Add(new CacheEntry(element) {Entry = temp});
        }

        [NotNull]
        private IList<TType> CreateList<TType>([CanBeNull] string itemType, [CanBeNull]params string[] nontemType)
            where TType : ItemEntryBase, new()
        {
            var list = new ItemEntryBaseList<TType>(_mainElement, itemType, nontemType);
            
            list.Cleared += ListCleared;
            list.Removed += ListRemoved;
            list.ItemSwitched += ListItemSwitched;

            return list;
        }

        private void ListItemSwitched([NotNull] object sender, [NotNull] ItemSwitchArgs itemSwitchArgs)
        {
            CacheEntry entry = _externalCache.Find(ent => Equals(ent.Entry, itemSwitchArgs.Switched));
            entry.Entry = itemSwitchArgs.Value;

            entry.Entry.SetItem(entry.Element);
        }

        private void ListRemoved([NotNull] object sender, [NotNull] ValueEventArgs valueEventArgs)
        {
            _externalCache.Remove(_externalCache.Find(ent => Equals(ent.Entry, valueEventArgs.Value)));
        }

        private void ListCleared([NotNull] object sender, [NotNull] ItemListCleared args)
        {
            foreach (var temp in args.OldValues
                .Select(entryBase => _externalCache.Find(ent => Equals(ent.Entry, entryBase)))
                .Where(ent => ent != null))
            {
                _externalCache.Remove(temp);
            }
        }
    }
}

