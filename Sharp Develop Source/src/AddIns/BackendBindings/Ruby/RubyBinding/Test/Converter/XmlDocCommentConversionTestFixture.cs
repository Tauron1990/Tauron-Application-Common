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
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class XmlDocCommentConversionTestFixture
	{
		string csharp = "/// <summary>\r\n" +
						"/// Class Foo\r\n" +
						"/// </summary>\r\n" +
						"public class Foo\r\n" +
						"{\r\n" +
						"    /// <summary>\r\n" +
						"    /// Run\r\n" +
						"    /// </summary>\r\n" +
						"    public void Run()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    /// <summary> Stop </summary>\r\n" +
						"    public void Stop()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    /// <summary> Initialize.</summary>\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        /// Initialize j.\r\n" +
						"        int j = 0; /// Set to zero\r\n" +
						"        /// test\r\n" +
						"        if (j == 0) j = 2;\r\n" +
						"    }\r\n" +
						"}";
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"# <summary>\r\n" +
				"# Class Foo\r\n" +
				"# </summary>\r\n" +
				"class Foo\r\n" +
				"    # <summary>\r\n" +
				"    # Run\r\n" +
				"    # </summary>\r\n" +
				"    def Run()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    # <summary> Stop </summary>\r\n" +
				"    def Stop()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    # <summary> Initialize.</summary>\r\n" +
				"    def initialize()\r\n" +
				"        # Initialize j.\r\n" +
				"        j = 0 # Set to zero\r\n" +
				"        # test\r\n" +
				"        if j == 0 then\r\n" +
				"            j = 2\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";

			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}
