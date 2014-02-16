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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Serializes object trees in a MycroXaml compatible way.
	/// </summary>
	public class MycroWriter
	{
		protected virtual string GetTypeName(Type t)
		{
			return t.Name;
		}
		
		public void Save(object obj, XmlWriter writer)
		{
			
			Type t = obj.GetType();
			writer.WriteStartElement(GetTypeName(t));
			foreach (PropertyInfo info in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (!info.CanRead) continue;
				if (info.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length != 0) continue;
				object val = info.GetValue(obj, null);
				if (val == null) continue;
				if (val is ICollection) {
					writer.WriteStartElement(info.Name);
					foreach (object element in (ICollection)val) {
						Save(element, writer);
					}
					writer.WriteEndElement();
				} else {
					if (!info.CanWrite) continue;
					TypeConverter c = TypeDescriptor.GetConverter(info.PropertyType);
					if (c.CanConvertFrom(typeof(string)) && c.CanConvertTo(typeof(string))) {
						try {
							writer.WriteElementString(info.Name, c.ConvertToInvariantString(val));
						} catch (Exception ex) {
							writer.WriteComment(ex.ToString());
						}
					} else if (info.PropertyType == typeof(Type)) {
						writer.WriteElementString(info.Name, ((Type)val).AssemblyQualifiedName);
					} else {
						writer.WriteStartElement(info.Name);
						Save(val, writer);
						writer.WriteEndElement();
					}
				}
			}
			writer.WriteEndElement();
		}
	}
}