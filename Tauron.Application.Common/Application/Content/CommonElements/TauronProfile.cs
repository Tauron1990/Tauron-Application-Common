using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class TauronProfile : ObservableObject, IEnumerable<string>
    {
        private static readonly char[] ContentSplitter = {'='};
        
        protected TauronProfile([NotNull] string application, [NotNull] string defaultPath)
        {
            Application = Argument.NotNull(application, nameof(application));
            _defaultPath = Argument.NotNull(defaultPath, nameof(defaultPath));
            LogCategory = "Tauron Profile";
        }
        
        public virtual string this[[NotNull] string key]
        {
            get => _settings[key];

            set
            {
                IlligalCharCheck(key);
                _settings[key] = value;
            }
        }

        public IEnumerator<string> GetEnumerator() => _settings.Select(k => k.Key).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly string _defaultPath;
        
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
        
        public int Count => _settings.Count;
        
        [NotNull]
        public string Application { get; private set; }
        
        [CanBeNull]
        public string Name { get; private set; }
        
        [CanBeNull]
        protected string Dictionary { get; private set; }
        
        [CanBeNull]
        protected string FilePath { get; private set; }
        
        public void Delete()
        {
            _settings.Clear();

            Log.Write("Delete Profile infos... " + Dictionary?.PathShorten(20), LogLevel.Info);

            Dictionary?.DeleteDirectory();
        }
        
        public virtual void Load([NotNull] string name)
        {
            Argument.NotNull(name, nameof(name));
            IlligalCharCheck(name);

            Name = name;
            Dictionary = _defaultPath.CombinePath(Application, name);
            Dictionary.CreateDirectoryIfNotExis();
            FilePath = Dictionary.CombinePath("Settings.db");

            Log.Write("Begin Load Profile infos... " + FilePath.PathShorten(20), LogLevel.Info);

            _settings.Clear();
            foreach (var vals in
                FilePath.EnumerateTextLinesIfExis()
                    .Select(line => line.Split(ContentSplitter, 2))
                    .Where(vals => vals.Length == 2))
            {
                Log.Write(LogLevel.Info, "key: {0} | Value {1}", vals[0], vals[1]);

                _settings[vals[0]] = vals[1];
            }
        }
        
        public virtual void Save()
        {
            Log.Write("Begin Save Profile infos...", LogLevel.Info);

            using (var writer = FilePath?.OpenTextWrite())
            {
                if(writer == null) return;

                foreach (var pair in _settings)
                {
                    writer.WriteLine("{0}={1}", pair.Key, pair.Value);

                    Log.Write(LogLevel.Info, "key: {0} | Value {1}", pair.Key, pair.Value);
                }
            }
        }
        
        [NotNull]
        public virtual string GetValue([NotNull] string key, [NotNull] string defaultValue)
        {
            Argument.NotNull(key, nameof(key));
            Argument.NotNull((object)defaultValue, nameof(defaultValue));

            IlligalCharCheck(key);

            return !_settings.ContainsKey(key) ? defaultValue : _settings[key];
        }
        
        public virtual void SetVaue([NotNull] string key, [NotNull] object value)
        {
            Argument.NotNull(key, nameof(key));
            Argument.NotNull(value, nameof(value));
            IlligalCharCheck(key);

            _settings[key] = value.ToString();
            OnPropertyChangedExplicit(key);
        }

        private void IlligalCharCheck([NotNull] string key) => Argument.Check(key.Contains('='), () => new ArgumentException($"The Key ({key}) Contains an Illigal Char: ="));
    }
}