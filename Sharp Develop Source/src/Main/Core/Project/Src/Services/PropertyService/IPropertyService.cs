﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;
using System.ComponentModel;
using Tauron.JetBrains.Annotations;

namespace ICSharpCode.Core
{
	/// <summary>
	/// The property service.
	/// </summary>
	[SDService("SD.PropertyService"), PublicAPI]
	public interface IPropertyService : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the configuration directory. (usually "%ApplicationData%\%ApplicationName%")
		/// </summary>
		[NotNull]
		DirectoryName ConfigDirectory { get; }
		
		/// <summary>
		/// Gets the data directory (usually "ApplicationRootPath\data")
		/// </summary>
		[NotNull]
		DirectoryName DataDirectory { get; }
		
		/// <summary>
		/// Gets the main properties container for this property service.
		/// </summary>
		[NotNull]
		Properties MainPropertiesContainer { get; }
		
		/// <inheritdoc cref="Properties.Get{T}(string, T)"/>
		T Get<T>([NotNull] string key, T defaultValue);
		
		/// <inheritdoc cref="Properties.NestedProperties"/>
		[NotNull]
		Properties NestedProperties([NotNull] string key);
		
		/// <inheritdoc cref="Properties.SetNestedProperties"/>
		void SetNestedProperties([NotNull] string key, [NotNull] Properties nestedProperties);
		
		/// <inheritdoc cref="Properties.Contains"/>
		bool Contains([NotNull] string key);
		
		/// <inheritdoc cref="Properties.Set{T}(string, T)"/>
		void Set<T>([NotNull] string key, T value);
		
		/// <inheritdoc cref="Properties.GetList{T}"/>
		[NotNull]
		IReadOnlyList<T> GetList<T>([NotNull] string key);
		
		/// <inheritdoc cref="Properties.SetList{T}"/>
		void SetList<T>([NotNull] string key, [NotNull] IEnumerable<T> value);
		
		/// <inheritdoc cref="Properties.Remove"/>
		void Remove([NotNull] string key);
		
		/// <summary>
		/// Saves the main properties to disk.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Loads extra properties that are not part of the main properties container.
		/// Unlike <see cref="NestedProperties"/>, multiple calls to <see cref="LoadExtraProperties"/>
		/// will return different instances, as the properties are re-loaded from disk every time.
		/// To save the properties, you need to call <see cref="SaveExtraProperties"/>.
		/// </summary>
		/// <returns>Properties container that was loaded; or an empty properties container
		/// if no container with the specified key exists.</returns>
		[NotNull]
		Properties LoadExtraProperties([NotNull] string key);
		
		/// <summary>
		/// Saves extra properties that are not part of the main properties container.
		/// </summary>
		void SaveExtraProperties([NotNull] string key, [NotNull] Properties p);
	}
}
