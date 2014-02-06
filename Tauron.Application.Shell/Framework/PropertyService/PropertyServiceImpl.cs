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
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using ICSharpCode.Core;
using Tauron.Application.Shell.Framework;

namespace ICSharpCode.Core
{
	public class PropertyServiceImpl : IPropertyService
	{
		readonly PropertiesContainer properties;
		
		/// <summary>
		/// Initializes the service for unit-testing (reset properties to an empty property container).
		/// Use <c>SD.InitializeForUnitTests()</c> instead, that initializes the property service and more.
		/// </summary>
		public PropertyServiceImpl()
		{
			properties = new PropertiesContainer();
		}
		
		public PropertyServiceImpl(PropertiesContainer properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.properties = properties;
		}
		
		public virtual DirectoryName ConfigDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		public virtual DirectoryName DataDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		/// <inheritdoc cref="PropertiesContainer.Get{T}(string, T)"/>
		public T Get<T>(string key, T defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		[Obsolete("Use the NestedProperties method instead", true)]
		public PropertiesContainer Get(string key, PropertiesContainer defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		/// <inheritdoc cref="PropertiesContainer.NestedProperties"/>
		public PropertiesContainer NestedProperties(string key)
		{
			return properties.NestedProperties(key);
		}
		
		/// <inheritdoc cref="PropertiesContainer.SetNestedProperties"/>
		public void SetNestedProperties(string key, PropertiesContainer nestedProperties)
		{
			properties.SetNestedProperties(key, nestedProperties);
		}
		
		/// <inheritdoc cref="PropertiesContainer.Contains"/>
		public bool Contains(string key)
		{
			return properties.Contains(key);
		}
		
		/// <inheritdoc cref="PropertiesContainer.Set{T}(string, T)"/>
		public void Set<T>(string key, T value)
		{
			properties.Set(key, value);
		}
		
		/// <inheritdoc cref="PropertiesContainer.GetList"/>
		public IReadOnlyList<T> GetList<T>(string key)
		{
			return properties.GetList<T>(key);
		}
		
		/// <inheritdoc cref="PropertiesContainer.SetList"/>
		public void SetList<T>(string key, IEnumerable<T> value)
		{
			properties.SetList(key, value);
		}
		
		/// <inheritdoc cref="PropertiesContainer.Remove"/>
		public void Remove(string key)
		{
			properties.Remove(key);
		}
		
		public event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
		
		public PropertiesContainer MainPropertiesContainer {
			get { return properties; }
		}
		
		public virtual void Save()
		{
		}

		public virtual PropertiesContainer LoadExtraProperties(string key)
		{
			return new PropertiesContainer();
		}

		public virtual void SaveExtraProperties(string key, PropertiesContainer p)
		{
		}
	}
}
