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
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests attribute refs
	/// </summary>
	[TestFixture]
	public class EnumAttributeValueTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection attributeValues;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("foo", "http://foo.com"));
			attributeValues = SchemaCompletion.GetAttributeValueCompletion(path, "id");
		}
		
		[Test]
		public void IdAttributeHasValueOne()
		{
			Assert.IsTrue(attributeValues.Contains("one"),
			              "Missing attribute value 'one'");
		}
		
		[Test]
		public void IdAttributeHasValueTwo()
		{
			Assert.IsTrue(attributeValues.Contains("two"),
			              "Missing attribute value 'two'");
		}		
		
		[Test]
		public void IdAttributeValueCount()
		{
			Assert.AreEqual(2, attributeValues.Count, "Expecting 2 attribute values.");
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://foo.com\" targetNamespace=\"http://foo.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attribute name=\"id\">\r\n" +
				"\t\t\t\t<xs:simpleType>\r\n" +
				"\t\t\t\t\t<xs:restriction base=\"xs:string\">\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"one\"/>\r\n" +
				"\t\t\t\t\t\t<xs:enumeration value=\"two\"/>\r\n" +
				"\t\t\t\t\t</xs:restriction>\r\n" +
				"\t\t\t\t</xs:simpleType>\r\n" +
				"\t\t\t</xs:attribute>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
