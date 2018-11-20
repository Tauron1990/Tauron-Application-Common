using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Controls;

namespace Tauron.Application.Help.ViewModels
{
    [PublicAPI]
    public sealed class HelpTopic : DependencyObject
    {
        internal HelpTopic([NotNull] string name, [NotNull] string tocken)
        {
            Tocken = Argument.NotNull(tocken, nameof(tocken));
            Name = Argument.NotNull(name, nameof(name));
        }

        public override string ToString() => Name;

        [NotNull]
        public string Name { get; private set; }

        [CanBeNull]
        public string Tocken
        {
            get => (string) ToggleButtonList.GetTopic(this);
            private set
            {
                if (value != null)
                    ToggleButtonList.SetTopic(this, value);
            }
        }
    }

    public sealed class HelpGroup : IHeaderProvider
    {
        public HelpGroup([NotNull] string titel, [NotNull] string content, [NotNull] string id)
        {
            Id = Argument.NotNull(id, nameof(id));
            _titel = Argument.NotNull(titel, nameof(titel));
            _content = Argument.NotNull(content, nameof(content));
        }

        private readonly string _content;

        private readonly string _titel;

        [CanBeNull]
        public FlowDocument Content => XamlReader.Parse(_content) as FlowDocument;

        [NotNull]
        public string Id { get; }

        [NotNull]
        public object Header => _titel;
    }

    public class HelpDatabase
    {
        private readonly XElement _database;

        public HelpDatabase([NotNull] string fileName)
        {
            if (fileName.ExisFile()) _database = XElement.Load(fileName);
            else if (fileName.ExisDirectory())
            {
                foreach (var file in fileName.GetFiles())
                {
                    _database = new XElement("dataBase");

                    var ele = XElement.Load(file);
                    foreach (var element in ele.Elements()) _database.Add(element);
                }
            }
            else _database = new XElement("help");
        }

        [NotNull]
        public IEnumerable<HelpTopic> Topics => from ele in _database.Elements()
            select new HelpTopic(ele.Attribute("Name")?.Value ?? string.Empty, ele.Attribute("Tocken")?.Value ?? string.Empty);

        public void Fill([NotNull] ICollection<HelpGroup> groups, [CanBeNull] string token)
        {
            var groupsContent =
                _database.Elements().FirstOrDefault(ele => ele.Attribute("Tocken")?.Value == token);
            if (groupsContent == null) return;

            foreach (var group in groupsContent.Elements())
                groups.Add(
                    new HelpGroup(
                        group.Attribute("Name")?.Value ?? string.Empty,
                        group.Elements().First().ToString(),
                        group.Attribute("Id")?.Value ?? string.Empty));
        }

    }

    [PublicAPI]
    public sealed class HelpViewModel : ObservableObject
    {
        public HelpViewModel([NotNull] string helpfilePath)
        {
            _groups = new ObservableCollection<HelpGroup>();
            _database = new HelpDatabase(Argument.NotNull(helpfilePath, nameof(helpfilePath)));

            Height = SystemParameters.MaximumWindowTrackHeight / 3 * 2;
            Width = SystemParameters.MaximumWindowTrackWidth / 3 * 2;
        }

        public void Activate([CanBeNull] string topic, [CanBeNull] string group)
        {
            if (string.IsNullOrWhiteSpace(topic)) return;

            var htopic = HelpTopics.FirstOrDefault(top => top.Tocken == topic);
            ActiveTopic = htopic;
            if (ActiveTopic == null) return;

            if (string.IsNullOrWhiteSpace(group)) return;

            var hGroup = HelpGroups.FirstOrDefault(gr => gr.Id == group);
            ActiveGroup = hGroup;
        }

        private readonly HelpDatabase _database;
        private readonly ObservableCollection<HelpGroup> _groups;
        private HelpGroup _activeGroup;
        private HelpTopic _activeTopic;
        private double _height;
        private double _width;

        [CanBeNull]
        public HelpGroup ActiveGroup
        {
            get => _activeGroup;

            private set
            {
                _activeGroup = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        public HelpTopic ActiveTopic
        {
            get => _activeTopic;

            private set
            {
                _activeTopic = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => _height;

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public IEnumerable<HelpGroup> HelpGroups => _groups;
        [NotNull]
        public IEnumerable<HelpTopic> HelpTopics => _database.Topics;

        public double Width
        {
            get => _width;

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        [EventTarget]
        private void NewTopic([NotNull] TopicChangedEvntArgs e) => _database.Fill(_groups, (string) e.Topic);

        [EventTarget]
        private void TopicClear() => _groups.Clear();
    }
}