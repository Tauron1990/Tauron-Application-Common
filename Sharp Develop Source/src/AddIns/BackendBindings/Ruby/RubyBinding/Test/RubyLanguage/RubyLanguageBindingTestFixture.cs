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
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace RubyBinding.Tests.RubyLanguage
{
	[TestFixture]
	public class RubyLanguageBindingTestFixture
	{
		RubyLanguageBinding languageBinding;
		
		[SetUp]
		public void Init()
		{
			languageBinding = new RubyLanguageBinding();
		}
		
		[Test]
		public void FormattingStrategyIsRubyFormattingStrategy()
		{
			Assert.IsInstanceOf(typeof(RubyFormattingStrategy), languageBinding.FormattingStrategy);
		}
		
		[Test]
		public void ImplementsILanguageBinding()
		{
			Assert.IsNotNull(languageBinding as ILanguageBinding);
		}
		
		[Test]
		public void LanguagePropertiesIsRubyLanguageProperties()
		{
			Assert.IsInstanceOf(typeof(RubyLanguageProperties), languageBinding.Properties);
		}
	}
}