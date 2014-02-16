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
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunnerApplication
	{
		string testResultsFileName = String.Empty;
		RubyAddInOptions options;
		RubyTestRunnerResponseFile responseFile;
		IScriptingFileService fileService;
		CreateTextWriterInfo textWriterInfo;
		RubyConsoleApplication consoleApplication;
		StringBuilder arguments;

		public RubyTestRunnerApplication(string testResultsFileName,
			RubyAddInOptions options,
			IScriptingFileService fileService)
		{
			this.testResultsFileName = testResultsFileName;
			this.options = options;
			this.fileService = fileService;
			consoleApplication = new RubyConsoleApplication(options);
		}
		
		public bool Debug {
			get { return consoleApplication.Debug; }
			set { consoleApplication.Debug = value; }
		}
		
		public void CreateResponseFile(SelectedTests selectedTests)
		{
			CreateResponseFile();
			using (responseFile) {
				WriteTests(selectedTests);
			}
		}
		
		void CreateResponseFile()
		{
			TextWriter writer = CreateTextWriter();
			responseFile = new RubyTestRunnerResponseFile(writer);
		}
		
		TextWriter CreateTextWriter()
		{
			string fileName = fileService.GetTempFileName();
			textWriterInfo = new CreateTextWriterInfo(fileName, Encoding.ASCII, false);
			return fileService.CreateTextWriter(textWriterInfo);
		}
		
		void WriteTests(SelectedTests selectedTests)
		{
			responseFile.WriteTests(selectedTests);
		}
		
		public ProcessStartInfo CreateProcessStartInfo(SelectedTests selectedTests)
		{
			consoleApplication.ScriptFileName = GetSharpDevelopTestRubyScriptFileName();
			AddLoadPaths(selectedTests.Project);
			consoleApplication.ScriptCommandLineArguments = GetCommandLineArguments(selectedTests);
			consoleApplication.WorkingDirectory = selectedTests.Project.Directory;
			return consoleApplication.GetProcessStartInfo();
		}
		
		void AddLoadPaths(IProject project)
		{
			AddLoadPathForRubyStandardLibrary();
			AddLoadPathForReferencedProjects(project);
		}
		
		void AddLoadPathForRubyStandardLibrary()
		{
			if (options.HasRubyLibraryPath) {
				consoleApplication.AddLoadPath(options.RubyLibraryPath);
			}
			string testRunnerLoadPath = Path.GetDirectoryName(consoleApplication.ScriptFileName);
			consoleApplication.AddLoadPath(testRunnerLoadPath);
		}
		
		void AddLoadPathForReferencedProjects(IProject project)
		{
			foreach (ProjectItem item in project.Items) {
				ProjectReferenceProjectItem projectRef = item as ProjectReferenceProjectItem;
				if (projectRef != null) {
					string directory = Path.GetDirectoryName(projectRef.FileName);
					consoleApplication.AddLoadPath(directory);
				}
			}
		}
		
		string GetSharpDevelopTestRubyScriptFileName()
		{
			string fileName = StringParser.Parse(@"${addinpath:ICSharpCode.RubyBinding}\TestRunner\sdtest.rb");
			return Path.GetFullPath(fileName);
		}
		
		string GetCommandLineArguments(SelectedTests selectedTests)
		{
			arguments = new StringBuilder();
			AppendSelectedTest(selectedTests);
			AppendTestResultsFileNameAndResponseFileNameArgs();
			
			return arguments.ToString();
		}
				
		void AppendSelectedTest(SelectedTests selectedTests)
		{
			if (selectedTests.Member != null) {
				AppendSelectedTestMethod(selectedTests.Member);
			} else if (selectedTests.Class != null) {
				AppendSelectedTestClass(selectedTests.Class);
			}
		}
		
		void AppendSelectedTestMethod(IMember method)
		{
			arguments.AppendFormat("--name={0} ", method.Name);
		}
		
		void AppendSelectedTestClass(IClass c)
		{
			arguments.AppendFormat("--testcase={0} ", c.FullyQualifiedName);
		}
		
		void AppendTestResultsFileNameAndResponseFileNameArgs()
		{
			arguments.AppendFormat("-- \"{0}\" \"{1}\"", testResultsFileName, textWriterInfo.FileName);
		}
		
		public void Dispose()
		{
			fileService.DeleteFile(textWriterInfo.FileName);
		}
	}
}