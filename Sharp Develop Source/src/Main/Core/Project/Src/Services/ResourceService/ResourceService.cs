// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using Tauron.JetBrains.Annotations;

namespace ICSharpCode.Core
{
    /// <summary>
    /// Compatibility class; forwards calls to the IResourceService.
    /// Remove
    /// </summary>
    public static class ResourceService
    {
        static IResourceService Service
        {
            get { return ServiceSingleton.GetRequiredService<IResourceService>(); }
        }

        public static string GetString(string resourceName)
        {
            return Service.GetString(resourceName);
        }

        public static string Language
        {
            get { return Service.Language; }
        }
    }

    /// <summary>
    /// This Class contains two ResourceManagers, which handle string and image resources
    /// for the application. It do handle localization strings on this level.
    /// </summary>
    public class ResourceServiceImpl : IResourceService
    {
        private const string UILanguageProperty = "CoreProperties.UILanguage";

        private const string StringResources = "StringResources";
        private const string ImageResources = "BitmapResources";

        private string _resourceDirectory;
        private IPropertyService _propertyService;

        public ResourceServiceImpl([NotNull] string resourceDirectory, [NotNull] IPropertyService propertyService)
        {
            if (resourceDirectory == null) throw new ArgumentNullException("resourceDirectory");
            if (propertyService == null) throw new ArgumentNullException("propertyService");

            _resourceDirectory = resourceDirectory;
            _propertyService = propertyService;
            propertyService.PropertyChanged += OnPropertyChange;
            LoadLanguageResources(Language);
        }

        public string Language
        {
            get
            {
                return _propertyService.Get(UILanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
            }
            set
            {
                if (Language != value)
                {
                    _propertyService.Set(UILanguageProperty, value);
                }
            }
        }

        private readonly object _loadLock = new object();

        /// <summary>English strings (list of resource managers)</summary>
        private List<ResourceManager> _strings = new List<ResourceManager>();

        /// <summary>Neutral/English images (list of resource managers)</summary>
        private List<ResourceManager> _icons = new List<ResourceManager>();

        /// <summary>Hashtable containing the local strings from the main application.</summary>
        private Hashtable _localStrings;

        private Hashtable _localIcons;

        /// <summary>Strings resource managers for the current language</summary>
        private List<ResourceManager> _localStringsResMgrs = new List<ResourceManager>();

        /// <summary>Image resource managers for the current language</summary>
        private List<ResourceManager> _localIconsResMgrs = new List<ResourceManager>();

        /// <summary>List of ResourceAssembly</summary>
        private List<ResourceAssembly> _resourceAssemblies = new List<ResourceAssembly>();

        private class ResourceAssembly
        {
            private readonly ResourceServiceImpl _service;
            private readonly Assembly _assembly;
            private readonly string _baseResourceName;
            private readonly bool _isIcons;

            public ResourceAssembly([NotNull] ResourceServiceImpl service, [NotNull] Assembly assembly, [NotNull] string baseResourceName,
                                    bool isIcons)
            {
                _service = service;
                _assembly = assembly;
                _baseResourceName = baseResourceName;
                _isIcons = isIcons;
            }

            [CanBeNull]
            private ResourceManager TrySatellite([NotNull] string language)
            {
                // ResourceManager should automatically use satellite assemblies, but it doesn't work
                // and we have to do it manually.
                string fileName = Path.GetFileNameWithoutExtension(_assembly.Location) + ".resources.dll";
                fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(_assembly.Location), language), fileName);
                
                if (!File.Exists(fileName)) return null;
                
                LoggingService.Info("Loging resources " + _baseResourceName + " loading from satellite " + language);
                return new ResourceManager(_baseResourceName, Assembly.LoadFrom(fileName));
            }

            public void Load()
            {
                string currentLanguage = _service._currentLanguage;
                string logMessage = "Loading resources " + _baseResourceName + "." + currentLanguage + ": ";
                ResourceManager manager;
                if (_assembly.GetManifestResourceInfo(_baseResourceName + "." + currentLanguage + ".resources") != null)
                {
                    LoggingService.Info(logMessage + " loading from main assembly");
                    manager = new ResourceManager(_baseResourceName + "." + currentLanguage, _assembly);
                }
                else if (currentLanguage.IndexOf('-') > 0
                         &&
                         _assembly.GetManifestResourceInfo(_baseResourceName + "." + currentLanguage.Split('-')[0] +
                                                          ".resources") != null)
                {
                    LoggingService.Info(logMessage + " loading from main assembly (no country match)");
                    manager = new ResourceManager(_baseResourceName + "." + currentLanguage.Split('-')[0], _assembly);
                }
                else
                {
                    // try satellite assembly
                    manager = TrySatellite(currentLanguage);
                    if (manager == null && currentLanguage.IndexOf('-') > 0)
                    {
                        manager = TrySatellite(currentLanguage.Split('-')[0]);
                    }
                }
                if (manager == null)
                {
                    LoggingService.Warn(logMessage + "NOT FOUND");
                }
                else
                {
                    if (_isIcons) _service._localIconsResMgrs.Add(manager);
                    else _service._localStringsResMgrs.Add(manager);
                }
            }
        }

        /// <summary>
        /// Registers string resources in the resource service.
        /// </summary>
        /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
        /// <param name="assembly">The assembly which contains the resource file.</param>
        /// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
        public void RegisterStrings(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralStrings(new ResourceManager(baseResourceName, assembly));
            var ra = new ResourceAssembly(this, assembly, baseResourceName, false);
            _resourceAssemblies.Add(ra);
            ra.Load();
        }

        public void RegisterNeutralStrings(ResourceManager stringManager)
        {
            _strings.Add(stringManager);
        }

        /// <summary>
        /// Registers image resources in the resource service.
        /// </summary>
        /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
        /// <param name="assembly">The assembly which contains the resource file.</param>
        /// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
        public void RegisterImages(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralImages(new ResourceManager(baseResourceName, assembly));
            var ra = new ResourceAssembly(this, assembly, baseResourceName, true);
            _resourceAssemblies.Add(ra);
            ra.Load();
        }

        public void RegisterNeutralImages(ResourceManager imageManager)
        {
            _icons.Add(imageManager);
        }

        private void OnPropertyChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            if (e.PropertyName != UILanguageProperty) return;

            LoadLanguageResources(Language);
            EventHandler handler = LanguageChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler LanguageChanged;
        private string _currentLanguage;

        private void LoadLanguageResources([NotNull] string language)
        {
            lock (_loadLock)
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                }
                catch (Exception)
                {
                    try
                    {
                        Thread.CurrentThread.CurrentUICulture =
                            new CultureInfo(language.Split('-')[0]);
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                }

                _localStrings = Load(StringResources, language);
                if (_localStrings == null && language.IndexOf('-') > 0) _localStrings = Load(StringResources, language.Split('-')[0]);

                _localIcons = Load(ImageResources, language);
                if (_localIcons == null && language.IndexOf('-') > 0) _localIcons = Load(ImageResources, language.Split('-')[0]);

                _localStringsResMgrs.Clear();
                _localIconsResMgrs.Clear();
                _currentLanguage = language;
                foreach (ResourceAssembly ra in _resourceAssemblies) ra.Load();
            }
        }

        [CanBeNull]
        private Hashtable Load([NotNull] string fileName)
        {
            if (!File.Exists(fileName)) return null;
            var resources = new Hashtable();
            var rr = new ResourceReader(fileName);
            foreach (DictionaryEntry entry in rr) resources.Add(entry.Key, entry.Value);
            rr.Close();
            return resources;
        }

        [CanBeNull]
        private Hashtable Load([NotNull] string name, [NotNull] string language)
        {
            return Load(_resourceDirectory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
        }

        /// <summary>
        /// Returns a string from the resource database, it handles localization
        /// transparent for the user.
        /// </summary>
        /// <returns>
        /// The string in the (localized) resource database.
        /// </returns>
        /// <param name="name">
        /// The name of the requested resource.
        /// </param>
        /// <exception cref="ResourceNotFoundException">
        /// Is thrown when the GlobalResource manager can't find a requested resource.
        /// </exception>
        public string GetString(string name)
        {
            lock (_loadLock)
            {
                if (_localStrings != null && _localStrings[name] != null)
                {
                    return _localStrings[name].ToString();
                }

                string s = null;
                foreach (ResourceManager resourceManger in _localStringsResMgrs)
                {
                    try
                    {
                        s = resourceManger.GetString(name);
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }

                    if (s != null)
                    {
                        break;
                    }
                }

                if (s == null)
                {
                    foreach (ResourceManager resourceManger in _strings)
                    {
                        try
                        {
                            s = resourceManger.GetString(name);
                        }
// ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                        }

                        if (s != null)
                        {
                            break;
                        }
                    }
                }
                if (s == null)
                {
                    throw new ResourceNotFoundException("string >" + name + "<");
                }

                return s;
            }
        }

        public object GetImageResource(string name)
        {
            lock (_loadLock)
            {
                object iconobj = null;
                if (_localIcons != null && _localIcons[name] != null)
                {
                    iconobj = _localIcons[name];
                }
                else
                {
                    foreach (ResourceManager resourceManger in _localIconsResMgrs)
                    {
                        iconobj = resourceManger.GetObject(name);
                        if (iconobj != null)
                        {
                            break;
                        }
                    }

                    if (iconobj != null) return iconobj;

                    foreach (ResourceManager resourceManger in _icons)
                    {
                        try
                        {
                            iconobj = resourceManger.GetObject(name);
                        }
// ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                        }

                        if (iconobj != null)
                        {
                            break;
                        }
                    }
                }
                return iconobj;
            }
        }
    }
}
