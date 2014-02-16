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
using System.Text;
using IronPython.Compiler;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportExpression : PythonExpression
	{
		string module = String.Empty;
		string identifier = String.Empty;
		bool hasFromAndImport;
		
		public PythonImportExpression(string expression)
			: this(Python.CreateEngine(), expression)
		{
		}
		
		public PythonImportExpression(ScriptEngine engine, string expression)
			: base(engine, expression)
		{
			Parse(engine, expression);
		}
		
		void Parse(ScriptEngine engine, string expression)
		{
			Token token = GetNextToken();
			if (IsImportToken(token)) {
				ParseImportExpression();
			} else if (IsFromToken(token)) {
				ParseFromExpression();
			}
		}
		
		void ParseImportExpression()
		{
			GetModuleName();
		}
		
		void ParseFromExpression()
		{
			GetModuleName();
			
			if (IsImportToken(CurrentToken)) {
				hasFromAndImport = true;
				GetIdentifierName();
			}
		}

		void GetModuleName()
		{
			module = GetName();
		}
		
		string GetName()
		{
			StringBuilder name = new StringBuilder();
			Token token = GetNextToken();
			while (IsNameToken(token)) {
				name.Append((string)token.Value);
				token = GetNextToken();
				if (IsDotToken(token)) {
					name.Append('.');
					token = GetNextToken();
				}
			}
			return name.ToString();
		}
		
		void GetIdentifierName()
		{
			identifier = GetName();
		}
		
		public bool HasFromAndImport {
			get { return hasFromAndImport; }
		}
		
		public string Module {
			get { return module; }
		}
		
		public string Identifier {
			get { return identifier; }
		}
		
		public bool HasIdentifier {
			get { return !String.IsNullOrEmpty(identifier); }
		}
		
		/// <summary>
		/// Returns true if the expression is of the form:
		/// 
		/// "import "
		/// "from "
		/// "import System"
		/// "from System"
		/// "from System import Console"
		/// </summary>
		public static bool IsImportExpression(string text, int offset)
		{
			if (!ValidIsImportExpressionParameters(text, offset)) {
				return false;
			}
			
			string previousWord = FindPreviousWord(text, offset);
			if (IsImportOrFromString(previousWord)) {
				return true;
			}
			
			int previousWordOffset = offset - previousWord.Length + 1;
			previousWord = FindPreviousWord(text, previousWordOffset);
			return IsImportOrFromString(previousWord);
		}
		
		static bool ValidIsImportExpressionParameters(string text, int offset)
		{
			if (String.IsNullOrEmpty(text) || (offset <= 0) || (offset >= text.Length)) {
				return false;
			}
			return true;
		}
		
		static string FindPreviousWord(string text, int offset)
		{
			int previousWordOffset = FindPreviousWordOffset(text, offset);
			int length = offset - previousWordOffset + 1;
			return text.Substring(previousWordOffset, length);
		}
		
		static int FindPreviousWordOffset(string text, int offset)
		{
			bool ignoreWhitespace = true;
			while (offset > 0) {
				char ch = text[offset];
				if (Char.IsWhiteSpace(ch)) {
					if (IsNewLineOrCarriageReturn(ch)) {
						return offset;
					}
					if (!ignoreWhitespace) {
						return offset;
					}
				} else {
					ignoreWhitespace = false;
				}
				--offset;
			}
			return offset;
		}
		
		static bool IsNewLineOrCarriageReturn(char ch)
		{
			return (ch == '\r') || (ch == '\n');
		}
		
		static bool IsImportOrFromString(string text)
		{
			return IsImportString(text) || IsFromString(text);
		}
		
		static bool IsImportString(string text)
		{
			return text.Trim() == "import";
		}
		
		static bool IsFromString(string text)
		{
			return text.Trim() == "from";
		}
	}
}