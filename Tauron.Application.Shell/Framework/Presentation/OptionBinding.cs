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
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Presentation
{
    /// <summary>
    /// Custom binding to allow direct bindings of option properties to WPF controls.
    /// </summary>
    /// <remarks>
    /// Properties accessed by this binding have to be managed by a custom
    /// settings class, which contains all settings as static properties or fields,
    /// or is a singleton class with the standard 'Instance' property.<br />
    /// Do not use PropertyService directly!<br />
    /// This markup extension can only be used in OptionPanels or other <br />containers implementing IOptionBindingContainer!
    /// </remarks>
    /// <example>
    /// <code>
    /// {sd:OptionBinding addin:XmlEditorAddInOptions.ShowAttributesWhenFolded}
    /// </code>
    /// <br />
    /// Whereas 'sd' is the xml namespace of ICSharpCode.Core.Presentation.OptionBinding and 'addin'<br />
    /// is the xml namespace, in which your settings class is defined.
    /// </example>
    [PublicAPI]
    public sealed class OptionBinding : MarkupExtension
    {
        private string _fullPropertyName;

        [CanBeNull]
        public string FullPropertyName
        {
            get { return _fullPropertyName; }
            set
            {
                if (!Regex.IsMatch(value))
                {
                    throw new ArgumentException(
                        "parameter must have the following format: namespace:ClassName.FieldOrProperty",
                        "propertyName");
                }

                _fullPropertyName = value;
            }
        }

        [CanBeNull]
        public IValueConverter Converter { get; set; }

        [CanBeNull]
        public object ConverterParameter { get; set; }

        [CanBeNull]
        public CultureInfo ConverterCulture { get; set; }

        private static readonly Regex Regex = new Regex("^.+\\:.+\\..+$", RegexOptions.Compiled);

        private DependencyObject _target;
        private DependencyProperty _dp;
        private bool _isStatic;
        private Type _propertyDeclaringType;
        private string _propertyName;

        private MemberInfo _propertyInfo;

        public OptionBinding([NotNull] string propertyName)
        {
            if (!Regex.IsMatch(propertyName))
            {
                throw new ArgumentException(
                    "parameter must have the following format: namespace:ClassName.FieldOrProperty",
                    "propertyName");
            }

            FullPropertyName = propertyName;
        }

        public OptionBinding([NotNull] Type container, [NotNull] string propertyName)
        {
            _propertyDeclaringType = container;
            _propertyName = propertyName;
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            var service = (IProvideValueTarget) provider.GetService(typeof (IProvideValueTarget));

            if (service == null) return null;

            _target = service.TargetObject as DependencyObject;
            _dp = service.TargetProperty as DependencyProperty;

            if (_target == null || _dp == null) return null;

            if (FullPropertyName != null)
            {
                string[] name = FullPropertyName.Split('.');
                var typeResolver = provider.GetService(typeof (IXamlTypeResolver)) as IXamlTypeResolver;
                if (typeResolver != null)
                {
                    _propertyDeclaringType = typeResolver.Resolve(name[0]);
                    _propertyName = name[1];
                }
            }

            _propertyInfo = _propertyDeclaringType.GetProperty(_propertyName);
            if (_propertyInfo != null)
            {
                _isStatic = (_propertyInfo as PropertyInfo).GetGetMethod().IsStatic;
            }
            else
            {
                _propertyInfo = _propertyDeclaringType.GetField(_propertyName);
                if (_propertyInfo != null)
                {
                    _isStatic = (_propertyInfo as FieldInfo).IsStatic;
                }
                else
                {
                    throw new ArgumentException("Could not find property " + _propertyName);
                }
            }

            var container = TryFindContainer(_target as FrameworkElement);

            if (container == null) throw new InvalidOperationException("This extension can be used in OptionPanels only!");

            container.AddBinding(this);

            object instance = _isStatic ? null : FetchInstance(_propertyDeclaringType);
            try
            {
                object result;

                if (_propertyInfo is PropertyInfo)
                {
                    result = (_propertyInfo as PropertyInfo).GetValue(instance, null);
                }
                else
                {
                    result = (_propertyInfo as FieldInfo).GetValue(instance);
                }

                return ConvertOnDemand(result, _dp.PropertyType);
            }
            catch (Exception e)
            {
                throw new Exception("Failing to convert " + FullPropertyName + " to " +
                                    _dp.OwnerType.Name + "." + _dp.Name + " (" + _dp.PropertyType + ")", e);
            }
        }

        /// <summary>
        /// Gets the 'Instance' from a singleton type.
        /// </summary>
        [NotNull]
        private static object FetchInstance([NotNull] Type type)
        {
            PropertyInfo instanceProp = type.GetProperty("Instance", type);
            if (instanceProp != null) return instanceProp.GetValue(null, null);
            FieldInfo instanceField = type.GetField("Instance");
            if (instanceField != null) return instanceField.GetValue(null);
            throw new ArgumentException("Type " + type.FullName +
                                        " has no 'Instance' property. Only singletons can be used with OptionBinding.");
        }

        [NotNull]
        private object ConvertOnDemand([NotNull] object result, [NotNull] Type returnType, bool convertBack = false)
        {
            if (Converter != null)
            {
                if (convertBack)
                {
                    result = Converter.ConvertBack(result, returnType, ConverterParameter,
                                                   ConverterCulture ?? CultureInfo.CurrentCulture);
                }
                else
                {
                    result = Converter.Convert(result, returnType, ConverterParameter,
                                               ConverterCulture ?? CultureInfo.CurrentCulture);
                }
            }
            if (returnType.IsInstanceOfType(result) || returnType == typeof (object)) return result;

            if (returnType == typeof (string))
            {
                var converter1 = TypeDescriptor.GetConverter(result.GetType());
                return converter1.ConvertToString(result);
            }

            if (!(result is string)) return Convert.ChangeType(result, returnType);
            var converter = TypeDescriptor.GetConverter(returnType);
            return converter.ConvertFromString(result as string);
        }

        [CanBeNull]
        private IOptionBindingContainer TryFindContainer([CanBeNull] DependencyObject start)
        {
            if (start == null) return null;

// ReSharper disable once SuspiciousTypeConversion.Global
            while (start != null && !(start is IOptionBindingContainer)) start = LogicalTreeHelper.GetParent(start);

// ReSharper disable once SuspiciousTypeConversion.Global
            return start as IOptionBindingContainer;
        }

        public bool Save()
        {
            object value = _target.GetValue(_dp);

            Type returnType = null;

            if (_propertyInfo is PropertyInfo) returnType = (_propertyInfo as PropertyInfo).PropertyType;
            if (_propertyInfo is FieldInfo) returnType = (_propertyInfo as FieldInfo).FieldType;

            if (returnType == null) return false;

            value = ConvertOnDemand(value, returnType, true);

            object instance = _isStatic ? null : FetchInstance(_propertyDeclaringType);
            if (_propertyInfo is PropertyInfo)
            {
                // Set only if the value is different from current value or default
                if (!(_propertyInfo as PropertyInfo).GetValue(instance, null).Equals(value))
                {
                    (_propertyInfo as PropertyInfo).SetValue(instance, value, null);
                }
                return true;
            }

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
// ReSharper disable once HeuristicUnreachableCode
            if (!(_propertyInfo is FieldInfo)) return false;
            // Set only if the value is different from current value or default
            if (!(_propertyInfo as FieldInfo).GetValue(instance).Equals(value))
            {
                (_propertyInfo as FieldInfo).SetValue(instance, value);
            }
            return true;
        }
    }
}
