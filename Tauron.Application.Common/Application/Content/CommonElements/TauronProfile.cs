#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The tauron profile.</summary>
    [PublicAPI]
    public abstract class TauronProfile : ObservableObject, IEnumerable<string>
    {
        #region Static Fields

        /// <summary>The content splitter.</summary>
        private static readonly char[] ContentSplitter = {'='};

        #endregion

        #region Fields

        /// <summary>The _default path.</summary>
        private readonly string _defaultPath;

        /// <summary>The _settings.</summary>
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        private string _application;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TauronProfile" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TauronProfile" /> Klasse.
        ///     Initializes a new instance of the <see cref="TauronProfile" /> class.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        /// <param name="dafaultPath">
        ///     The dafault path.
        /// </param>
        protected TauronProfile([NotNull] string application, [NotNull] string dafaultPath)
        {
            Contract.Requires<ArgumentNullException>(application != null, "application");
            Contract.Requires<ArgumentNullException>(dafaultPath != null, "_defaultPath");

            Application = application;
            _defaultPath = dafaultPath;
            Category = "Tauron Profile";
        }

        #endregion

        #region Public Properties

        public int Count { get { return _settings.Count; } }

        /// <summary>Gets the application.</summary>
        /// <value>The application.</value>
        [NotNull]
        public string Application
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _application;
            }

            private set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _application = value;
            }
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        [NotNull]
        public string Name { get; private set; }

        #endregion

        #region Properties

        /// <summary>Gets the dictionary.</summary>
        /// <value>The dictionary.</value>
        [NotNull]
        protected string Dictionary { get; private set; }

        /// <summary>Gets the file path.</summary>
        /// <value>The file path.</value>
        [NotNull]
        protected string FilePath { get; private set; }

        #endregion

        #region Indexers

        public virtual string this[[NotNull]string key]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(key != null, "key");

                return _settings[key];
            }

            set
            {
                Contract.Requires<ArgumentNullException>(key != null, "key");
                IlligalCharCheck(key);

                _settings[key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The delete.</summary>
        public void Delete()
        {
            _settings.Clear();

            Contract.Assume(Dictionary != null);

            Log.Write("Delete Profile infos... " + Dictionary.PathShorten(20), TraceEventType.Information);

            Dictionary.DeleteDirectory();
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        [ContractVerification(false)]
        public virtual void Load([NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            IlligalCharCheck(name);

            Name = name;
            Dictionary = _defaultPath.CombinePath(Application, name);
            Dictionary.CreateDirectoryIfNotExis();
            FilePath = Dictionary.CombinePath("Settings.db");

            Log.Write("Begin Load Profile infos... " + FilePath.PathShorten(20), TraceEventType.Information);

            _settings.Clear();
            foreach (var vals in
                FilePath.EnumerateTextLinesIfExis()
                        .Select(line => line.Split(ContentSplitter, 2))
                        .Where(vals => vals.Length == 2))
            {
                Log.Write(string.Format("key: {0} | Value {1}", vals[0], vals[1]), TraceEventType.Information);

                _settings[vals[0]] = vals[1];
            }
        }

        /// <summary>The save.</summary>
        public virtual void Save()
        {
            Log.Write("Begin Save Profile infos...", TraceEventType.Information);

            using (StreamWriter writer = FilePath.OpenTextWrite())
            {
                foreach (var pair in _settings)
                {
                    writer.WriteLine("{0}={1}", pair.Key, pair.Value);

                    Log.Write(string.Format("key: {0} | Value {1}", pair.Key, pair.Value), TraceEventType.Information);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get value.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public virtual string GetValue([NotNull] string key, [NotNull] string defaultValue)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");

            IlligalCharCheck(key);

            return !_settings.ContainsKey(key) ? defaultValue : _settings[key];
        }

        /// <summary>
        ///     The set vaue.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public virtual void SetVaue([NotNull] string key, [NotNull] object value)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");
            IlligalCharCheck(key);
            Contract.Requires<ArgumentNullException>(value != null, "value");

            _settings[key] = value.ToString();
            OnPropertyChangedExplicit(key);
        }

        [ContractAbbreviator]
        private void IlligalCharCheck([NotNull] string key)
        {
            Contract.Requires<InvalidOperationException>(!key.Contains("="), "The Contains an Illigal Char: =");
        }

        #endregion

        public IEnumerator<string> GetEnumerator()
        {
            return _settings.Select(k => k.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}