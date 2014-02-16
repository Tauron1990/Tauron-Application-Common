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
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Ruby.
	/// </summary>
	public class ConvertToRubyMenuCommand : AbstractMenuCommand
	{
		ScriptingTextEditorViewContent view;
		
		public override void Run()
		{
			Run(new RubyWorkbench());
		}
		
		protected void Run(IScriptingWorkbench workbench)
		{
			view = new ScriptingTextEditorViewContent(workbench);
			string code = GenerateRubyCode();
			ShowRubyCodeInNewWindow(code);
		}
		
		string GenerateRubyCode()
		{
			ParseInformation parseInfo = GetParseInformation(view.PrimaryFileName);
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(view.PrimaryFileName, parseInfo);
			converter.IndentString = view.TextEditorOptions.IndentationString;
			return converter.Convert(view.EditableView.Text);
		}
		
		void ShowRubyCodeInNewWindow(string code)
		{
			NewFile("Generated.rb", "Ruby", code);
		}
		
		protected virtual ParseInformation GetParseInformation(string fileName)
		{
			return ParserService.GetParseInformation(fileName);
		}
		
		/// <summary>
		/// Creates a new file using the FileService by default.
		/// </summary>
		protected virtual void NewFile(string defaultName, string language, string content)
		{
			FileService.NewFile(defaultName, content);
		}
	}
}