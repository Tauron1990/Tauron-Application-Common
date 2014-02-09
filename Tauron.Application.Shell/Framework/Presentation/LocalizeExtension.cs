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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Tauron.Application.Composition;
using Tauron.Application.Shell.Framework.Core.ResourceService;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Presentation
{
    /// <summary>
    /// Markup extension that retrieves localized resource strings.
    /// </summary>
    [MarkupExtensionReturnType(typeof (string))]
    [PublicAPI]
    public sealed class LocalizeExtension : LanguageDependentExtension
    {
        [NotNull]
        public static IResourceService ResourceService
        {
            get { return CompositionServices.Container.Resolve<IResourceService>(); }
        }

        public LocalizeExtension([NotNull] string key)
        {
            _key = key;
            UsesAccessors = true;
            UpdateOnLanguageChange = true;
        }

        public LocalizeExtension()
        {
            UsesAccessors = true;
            UpdateOnLanguageChange = true;
        }

        private string _key;

        [NotNull]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Set whether the text uses accessors.
        /// If set to true (default), accessors will be converted to WPF syntax.
        /// </summary>
        public bool UsesAccessors { get; set; }

        public override string Value
        {
            get
            {
                try
                {
                    string result = ResourceService.GetString(_key);
                    if (UsesAccessors) result = MenuService.ConvertLabel(result);
                    return result;
                }
                catch (ResourceNotFoundException)
                {
                    return "{Localize:" + _key + "}";
                }
            }
        }
    }

    [PublicAPI]
    public abstract class LanguageDependentExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
    {
        protected LanguageDependentExtension()
        {
            UpdateOnLanguageChange = true;
        }

        [NotNull]
        public abstract string Value { get; }

        /// <summary>
        /// Set whether the LocalizeExtension should use a binding to automatically
        /// change the text on language changes.
        /// The default value is true.
        /// </summary>
        public bool UpdateOnLanguageChange { get; set; }

        private bool _isRegisteredForLanguageChange;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!UpdateOnLanguageChange) return Value;
            var binding = new Binding("Value") {Source = this, Mode = BindingMode.OneWay};
            return binding.ProvideValue(serviceProvider);
        }

        private event PropertyChangedEventHandler ChangedEvent;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_isRegisteredForLanguageChange)
                {
                    _isRegisteredForLanguageChange = true;
                    LanguageChangeWeakEventManager.AddListener(this);
                }
                ChangedEvent += value;
            }
            remove { ChangedEvent -= value; }
        }

        public static readonly PropertyChangedEventArgs
            ValueChangedEventArgs = new PropertyChangedEventArgs("Value");

        bool IWeakEventListener.ReceiveWeakEvent([NotNull] Type managerType, [NotNull] object sender,
                                                 [NotNull] EventArgs e)
        {
            var handler = ChangedEvent;
            if (handler != null) handler(this, ValueChangedEventArgs);
            return true;
        }
    }
}
