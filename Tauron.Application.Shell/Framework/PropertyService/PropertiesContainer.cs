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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;
using Tauron.Application.Files.VirtualFiles;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework
{
    /// <summary>
    /// This interface flags an object beeing "mementocapable". This means that the
    /// state of the object could be saved to an <see cref="PropertiesContainer"/> object
    /// and set from a object from the same class.
    /// This is used to save and restore the state of GUI objects.
    /// </summary>
    /// <remarks>
    /// This interface is used as a [ViewContentService]
    /// </remarks>
    [PublicAPI]
    public interface IMementoCapable
    {
        /// <summary>
        /// Creates a new memento from the state.
        /// </summary>
        [NotNull]
        PropertiesContainer CreateMemento();

        /// <summary>
        /// Sets the state to the given memento.
        /// </summary>
        void SetMemento([NotNull] PropertiesContainer memento);
    }

    /// <summary>
    /// A container for settings - key/value pairs where keys are strings, and values are arbitrary objects.
    /// Instances of this class are thread-safe.
    /// </summary>
    [PublicAPI]
    public sealed class PropertiesContainer : INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// Gets the version number of the XML file format.
        /// </summary>
        public static readonly Version FileVersion = new Version(2, 0, 0);

        // Properties instances form a tree due to the nested properties containers.
        // All nodes in such a tree share the same syncRoot in order to simplify synchronization.
        // When an existing node is added to a tree, its syncRoot needs to change.
        private object _syncRoot;
        private PropertiesContainer _parent;
        // Objects in the dictionary are one of:
        // - string: value stored using TypeConverter
        // - XElement: serialized object
        // - object[]: a stored list (array elements are null, string or XElement)
        // - Properties: nested properties container
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        #region Constructor

        public PropertiesContainer()
        {
            _syncRoot = new object();
        }

        private PropertiesContainer([NotNull] PropertiesContainer parent)
        {
            _parent = parent;
            _syncRoot = parent._syncRoot;
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([NotNull] string key)
        {
            var handler = Volatile.Read(ref PropertyChanged);
            if (handler != null) handler(this, new PropertyChangedEventArgs(key));
        }

        #endregion

        #region IsDirty

        private bool _isDirty;

        /// <summary>
        /// Gets/Sets whether this properties container is dirty.
        /// IsDirty automatically gets set to <c>true</c> when a property in this container (or a nested container)
        /// changes.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                lock (_syncRoot)
                {
                    if (value) MakeDirty();
                    else CleanDirty();
                }
            }
        }

        private void MakeDirty()
        {
            // called within syncroot
            if (_isDirty) return;
            _isDirty = true;
            if (_parent != null) _parent.MakeDirty();
        }

        private void CleanDirty()
        {
            if (!_isDirty) return;
            _isDirty = false;
            foreach (var properties in _dict.Values.OfType<PropertiesContainer>())
            {
                properties.CleanDirty();
            }
        }

        #endregion

        #region Keys/Contains

        /// <summary>
        /// Gets the keys that are in use by this properties container.
        /// </summary>
        [NotNull]
        public IReadOnlyList<string> Keys
        {
            get
            {
                lock (_syncRoot)
                {
                    return _dict.Keys.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets whether this properties instance contains any entry (value, list, or nested container)
        /// with the specified key.
        /// </summary>
        public bool Contains([NotNull] string key)
        {
            lock (_syncRoot)
            {
                return _dict.ContainsKey(key);
            }
        }

        #endregion

        #region Get and Set

        /// <summary>
        /// Retrieves a string value from this Properties-container.
        /// Using this indexer is equivalent to calling <c>Get(key, string.Empty)</c>.
        /// </summary>
        public string this[string key]
        {
            get
            {
                lock (_syncRoot)
                {
                    object val;
                    _dict.TryGetValue(key, out val);
                    return val as string ?? string.Empty;
                }
            }
            set
            {
                Set(key, value);
            }
        }

        /// <summary>
        /// Retrieves a single element from this Properties-container.
        /// </summary>
        /// <param name="key">Key of the item to retrieve</param>
        /// <param name="defaultValue">Default value to be returned if the key is not present.</param>
        public T Get<T>([NotNull] string key, T defaultValue)
        {
            lock (_syncRoot)
            {
                object val;
                if (!_dict.TryGetValue(key, out val)) return defaultValue;
                try
                {
                    return (T) Deserialize(val, typeof (T));
                }
                catch (SerializationException ex)
                {
                        
                    CommonShellConstans.LogCommon(false, ex.ToString());
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Sets a single element in this Properties-container.
        /// The element will be serialized using a TypeConverter if possible, or XAML serializer otherwise.
        /// </summary>
        /// <remarks>Setting a key to <c>null</c> has the same effect as calling <see cref="Remove"/>.</remarks>
        public void Set<T>([NotNull] string key, T value)
        {
            object serializedValue = Serialize(value, typeof (T), key);
            SetSerializedValue(key, serializedValue);
        }

        private void SetSerializedValue([NotNull] string key, [CanBeNull] object serializedValue)
        {
            if (serializedValue == null)
            {
                Remove(key);
                return;
            }
            lock (_syncRoot)
            {
                object oldValue;
                if (_dict.TryGetValue(key, out oldValue))
                {
                    if (Equals(serializedValue, oldValue)) return;
                    HandleOldValue(oldValue);
                }
                _dict[key] = serializedValue;
            }
            OnPropertyChanged(key);
        }

        #endregion

        #region GetList/SetList

        /// <summary>
        /// Retrieves the list of items stored with the specified key.
        /// If no entry with the specified key exists, this method returns an empty list.
        /// </summary>
        /// <remarks>
        /// This method returns a copy of the list used internally; you need to call
        /// <see cref="SetList{T}"/> if you want to store the changed list.
        /// </remarks>
        [NotNull]
        public IReadOnlyList<T> GetList<T>([NotNull] string key)
        {
            lock (_syncRoot)
            {
                object val;
                if (!_dict.TryGetValue(key, out val)) return new T[0];
                var serializedArray = val as object[];
                if (serializedArray != null)
                {
                    try
                    {
                        var array = new T[serializedArray.Length];
                        for (int i = 0; i < array.Length; i++) array[i] = (T) Deserialize(serializedArray[i], typeof (T));
                        return array;
                    }
                    catch (XamlObjectWriterException ex)
                    {
                        CommonShellConstans.LogCommon(false, ex.ToString());
                    }
                    catch (NotSupportedException ex)
                    {
                        CommonShellConstans.LogCommon(false, ex.ToString());
                    }
                }
                else CommonShellConstans.LogCommon(false, "Properties.GetList(" + key + ") - this entry is not a list");
                return new T[0];
            }
        }

        /// <summary>
        /// Sets a list of elements in this Properties-container.
        /// The elements will be serialized using a TypeConverter if possible, or XAML serializer otherwise.
        /// </summary>
        /// <remarks>Passing <c>null</c> or an empty list as value has the same effect as calling <see cref="Remove"/>.</remarks>
        public void SetList<T>([NotNull] string key, [CanBeNull] IEnumerable<T> value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null)
            {
                Remove(key);
                return;
            }
            T[] array = value.ToArray();
            if (array.Length == 0)
            {
                Remove(key);
                return;
            }
            var serializedArray = new object[array.Length];
            for (int i = 0; i < array.Length; i++) serializedArray[i] = Serialize(array[i], typeof (T), null);
            SetSerializedValue(key, serializedArray);
        }

        #endregion

        #region Serialization

        [CanBeNull]
        private object Serialize([CanBeNull] object value, [NotNull] Type sourceType, [CanBeNull] string key)
        {
            if (value == null) return null;
            TypeConverter c = TypeDescriptor.GetConverter(sourceType);
            if (c != null && c.CanConvertTo(typeof (string)) && c.CanConvertFrom(typeof (string))) return c.ConvertToInvariantString(value);

            var element = new XElement("SerializedObject");
            if (key != null) element.Add(new XAttribute("key", key));
            using (var xmlWriter = element.CreateWriter()) XamlServices.Save(xmlWriter, value);
            return element;
        }

        [CanBeNull]
        private object Deserialize([CanBeNull] object serializedVal, [NotNull] Type targetType)
        {
            if (serializedVal == null) return null;
            var element = serializedVal as XElement;
            if (element != null) using (var xmlReader = element.Elements().Single().CreateReader()) return XamlServices.Load(xmlReader);
            
            var text = serializedVal as string;
            
            if (text == null) throw new InvalidOperationException("Cannot read a properties container as a single value");
            var c = TypeDescriptor.GetConverter(targetType);
            return c.ConvertFromInvariantString(text);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes the entry (value, list, or nested container) with the specified key.
        /// </summary>
        public bool Remove([NotNull] string key)
        {
            bool removed = false;
            lock (_syncRoot)
            {
                object oldValue;
                if (_dict.TryGetValue(key, out oldValue))
                {
                    removed = true;
                    HandleOldValue(oldValue);
                    MakeDirty();
                    _dict.Remove(key);
                }
            }
            if (removed) OnPropertyChanged(key);
            return removed;
        }

        #endregion

        #region Nested Properties

        /// <summary>
        /// Gets the parent property container.
        /// </summary>
        [CanBeNull]
        public PropertiesContainer Parent
        {
            get
            {
                lock (_syncRoot)
                {
                    return _parent;
                }
            }
        }

        /// <summary>
        /// Retrieves a nested property container; creating a new one on demand.
        /// Multiple calls to this method will return the same instance (unless the entry at this key
        /// is overwritten by one of the Set-methods).
        /// Changes performed on the nested container will be persisted together with the parent container.
        /// </summary>
        [NotNull]
        public PropertiesContainer NestedProperties([NotNull] string key)
        {
            bool isNewContainer = false;
            PropertiesContainer result;
            lock (_syncRoot)
            {
                object oldValue;
                _dict.TryGetValue(key, out oldValue);
                result = oldValue as PropertiesContainer;
                if (result == null)
                {
                    result = new PropertiesContainer(this);
                    isNewContainer = true;
                    _dict[key] = result;
                    result.MakeDirty();
                }
            }
            if (isNewContainer) OnPropertyChanged(key);
            return result;
        }

        private void HandleOldValue([NotNull] object oldValue)
        {
            var p = oldValue as PropertiesContainer;
            if (p == null) return;
            Debug.Assert(p._parent == this);
            p._parent = null;
        }

        /// <summary>
        /// Attaches the specified properties container as nested properties.
        /// 
        /// This method is intended to be used in conjunction with the <see cref="IMementoCapable"/> pattern
        /// where a new unattached properties container is created and then later attached to a parent container.
        /// </summary>
        public void SetNestedProperties([NotNull] string key, [CanBeNull] PropertiesContainer properties)
        {
            if (properties == null)
            {
                Remove(key);
                return;
            }
            lock (_syncRoot)
            {
                for (PropertiesContainer ancestor = this; ancestor != null; ancestor = ancestor._parent) if (ancestor == properties) throw new InvalidOperationException("Cannot add a properties container to itself.");

                object oldValue;
                if (_dict.TryGetValue(key, out oldValue))
                {
                    if (oldValue == properties) return;
                    HandleOldValue(oldValue);
                }
                lock (properties._syncRoot)
                {
                    if (properties._parent != null)
                    {
                        throw new InvalidOperationException(
                            "Cannot attach nested properties that already have a parent.");
                    }
                    MakeDirty();
                    properties.SetSyncRoot(_syncRoot);
                    properties._parent = this;
                    _dict[key] = properties;
                }
            }
            OnPropertyChanged(key);
        }

        private void SetSyncRoot([NotNull] object newSyncRoot)
        {
            _syncRoot = newSyncRoot;
            foreach (var properties in _dict.Values.OfType<PropertiesContainer>()) properties.SetSyncRoot(newSyncRoot);
        }

        #endregion

        #region Clone

        /// <summary>
        /// Creates a deep clone of this Properties container.
        /// </summary>
        [NotNull]
        public PropertiesContainer Clone()
        {
            lock (_syncRoot)
            {
                return CloneWithParent(null);
            }
        }

        [NotNull]
        private PropertiesContainer CloneWithParent([CanBeNull] PropertiesContainer parent)
        {
            PropertiesContainer copy = parent != null ? new PropertiesContainer(parent) : new PropertiesContainer();
            foreach (var pair in _dict)
            {
                var child = pair.Value as PropertiesContainer;
                copy._dict.Add(pair.Key, child != null ? child.CloneWithParent(copy) : pair.Value);
            }
            return copy;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region ReadFromAttributes

        [NotNull]
        internal static PropertiesContainer ReadFromAttributes([NotNull] XmlReader reader)
        {
            var properties = new PropertiesContainer();
            
            if (!reader.HasAttributes) return properties;
            
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                // some values are frequently repeated (e.g. type="MenuItem"),
                // so we also use the NameTable for attribute values
                // (XmlReader itself only uses it for attribute names)
                string val = reader.NameTable.Add(reader.Value);
                properties[reader.Name] = val;
            }
            reader.MoveToElement(); //Moves the reader back to the element node.
            return properties;
        }

        #endregion

        #region Load/Save

        [NotNull]
        public static PropertiesContainer Load([NotNull] IFile fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            return Load(XDocument.Load(fileName.Open(FileAccess.Read)).Root);
        }

        [NotNull]
        public static PropertiesContainer Load([NotNull] XElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            var properties = new PropertiesContainer();
            properties.LoadContents(element.Elements());
            return properties;
        }

        private void LoadContents([NotNull] IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                var key = (string) element.Attribute("key");
                if (key == null) continue;
                switch (element.Name.LocalName)
                {
                    case "Property":
                        _dict[key] = element.Value;
                        break;
                    case "Array":
                        _dict[key] = LoadArray(element.Elements());
                        break;
                    case "SerializedObject":
                        _dict[key] = new XElement(element);
                        break;
                    case "Properties":
                        var child = new PropertiesContainer(this);
                        child.LoadContents(element.Elements());
                        _dict[key] = child;
                        break;
                }
            }
        }

        [NotNull]
        private static object[] LoadArray([NotNull] IEnumerable<XElement> elements)
        {
            var result = new List<object>();
            foreach (var element in elements)
            {
                switch (element.Name.LocalName)
                {
                    case "Null":
                        result.Add(null);
                        break;
                    case "Element":
                        result.Add(element.Value);
                        break;
                    case "SerializedObject":
                        result.Add(new XElement(element));
                        break;
                }
            }
            return result.ToArray();
        }

        public void Save([NotNull] IFile fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            new XDocument(Save()).Save(fileName.Open(FileAccess.Write));
        }

        [NotNull]
        public XElement Save()
        {
            lock (_syncRoot)
            {
                return new XElement("Properties", SaveContents());
            }
        }

        [NotNull]
        private IReadOnlyList<XElement> SaveContents()
        {
            var result = new List<XElement>();
            foreach (var pair in _dict)
            {
                var key = new XAttribute("key", pair.Key);
                var child = pair.Value as PropertiesContainer;
                if (child != null)
                {
                    var contents = child.SaveContents();
                    if (contents.Count > 0) result.Add(new XElement("Properties", key, contents));
                }
                else
                {
                    var value = pair.Value as object[];
                    if (value != null)
                    {
                        var array = value;
                        var elements = new XElement[array.Length];
                        for (int i = 0; i < array.Length; i++)
                        {
                            var obj = array[i] as XElement;
                            if (obj != null) elements[i] = new XElement(obj);
                            else if (array[i] == null) elements[i] = new XElement("Null");
                            else elements[i] = new XElement("Element", array[i]);
                        }
                        result.Add(new XElement("Array", key, elements));
                    }
                    else
                    {
                        var other = pair.Value as XElement;
                        result.Add(other != null
                                       ? new XElement(other)
                                       : new XElement("Property", key, (string) pair.Value));
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
