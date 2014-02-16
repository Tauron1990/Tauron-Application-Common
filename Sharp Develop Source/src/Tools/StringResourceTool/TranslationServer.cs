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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace StringResourceTool
{
	public class TranslationServer
	{
		TextBox output;
		string baseURL = "http://developer.sharpdevelop.net/corsavy/translation/";
		
		public TranslationServer(TextBox output)
		{
			this.output = output;
			this.cookieContainer = new CookieContainer();
			this.wc = new CookieAwareWebClient(cookieContainer);
		}
		
		CookieContainer cookieContainer;
		CookieAwareWebClient wc;
		
		public bool Login(string user, string pwd)
		{
			output.Text = "Contacting server...";
			Application.DoEvents();
			System.Threading.Thread.Sleep(50);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL + "logon.asp");
			request.ContentType = "application/x-www-form-urlencoded";
			string postString = "uname=" + user + "&upwd=" + HttpUtility.UrlEncode(pwd);
			request.ContentLength = postString.Length;
			request.CookieContainer = cookieContainer;
			request.Method = "POST";
			request.AllowAutoRedirect = false;
			Stream s = request.GetRequestStream();
			using (StreamWriter w = new StreamWriter(s)) {
				w.Write(postString);
			}
			s.Close();
			string result;
			using (StreamReader r = new StreamReader(request.GetResponse().GetResponseStream())) {
				result = r.ReadToEnd();
			}
			if (result.Contains("You couldn't be logged on")) {
				output.Text += "\r\nInvalid username/password.";
				return false;
			}
			output.Text += "\r\nLogin successful.";
			return true;
		}
		public void DownloadDatabase(string targetFile, EventHandler successCallback)
		{
			wc.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e) {
				output.BeginInvoke((MethodInvoker)delegate {
				                   	output.Text = "Download: " + e.ProgressPercentage + "%";
				                   });
			};
			wc.DownloadDataCompleted += delegate(object sender, DownloadDataCompletedEventArgs e) {
				output.BeginInvoke((MethodInvoker)delegate {
				                   	if (e.Error != null)
				                   		output.Text = e.Error.ToString();
				                   	else
				                   		output.Text = "Download complete.";
				                   });
				if (e.Error == null) {
					using (FileStream fs = new FileStream(targetFile, FileMode.Create, FileAccess.Write)) {
						fs.Write(e.Result, 0, e.Result.Length);
					}
					successCallback(this, EventArgs.Empty);
				}
				wc.Dispose();
			};
			wc.DownloadDataAsync(new Uri(baseURL + "CompactNdownload.asp"));
		}
		
		public void AddResourceString(string idx, string value, string purpose)
		{
			wc.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
			wc.UploadString(new Uri(baseURL + "owners_AddNew.asp"),
			                "Idx=" + Uri.EscapeDataString(idx)
			                + "&PrimaryResLangValue=" + Uri.EscapeDataString(value)
			                + "&PrimaryPurpose=" + Uri.EscapeDataString(purpose));
		}
		
		public void UpdateTranslation(string idx, string newValue)
		{
			newValue = HttpUtility.UrlEncode(newValue, Encoding.Default);
			wc.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
			wc.UploadString(new Uri(baseURL + "translation_edit.asp"),
			                "Idx=" + Uri.EscapeDataString(idx)
			                + "&Localization=" + newValue);
		}
		
		public void DeleteResourceStrings(string[] idx)
		{
			const int threadCount = 3; // 3 parallel calls
			output.Text = "Deleting...";
			int index = 0;
			int finishCount = 0;
			EventHandler callback = null;
			callback = delegate {
				lock (idx) {
					if (index < idx.Length) {
						DeleteResourceString(idx[index++], callback);
					} else {
						finishCount += 1;
						if (finishCount == threadCount) {
							output.BeginInvoke((MethodInvoker)delegate {
							                   	output.Text += "\r\nFinished.";
							                   	output.Text += "\r\nYou have to re-download the database to see the changes.";
							                   });
						}
					}
				}
			};
			for (int i = 0; i < threadCount; i++) {
				callback(null, null);
			}
		}
		
		public void DeleteResourceString(string idx, EventHandler callback)
		{
			wc.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
			wc.UploadStringCompleted += delegate {
				output.BeginInvoke((MethodInvoker)delegate {
				                   	output.Text += "\r\nDeleted " + idx;
				                   });
				wc.Dispose();
				if (callback != null)
					callback(this, EventArgs.Empty);
			};
			wc.UploadStringAsync(new Uri(baseURL + "owners_delete.asp"),
			                     "Idx=" + Uri.EscapeDataString(idx) + "&ReallyDelete=on");
		}
		
		public void SetLanguage(string language)
		{
			wc.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
			wc.UploadString(new Uri(baseURL + "SelectLanguage.asp"),
			                "Language=" + Uri.EscapeDataString(language));
		}
	}
}