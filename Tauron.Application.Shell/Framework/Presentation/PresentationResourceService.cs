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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Tauron.Application.Composition;
using Tauron.Application.Shell.Framework.Core.ResourceService;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Presentation
{
	/// <summary>
	/// Creates WPF BitmapSource objects from images in the ResourceService.
	/// </summary>
	[PublicAPI]
	public static class PresentationResourceService
	{
		static readonly Dictionary<string, BitmapSource> BitmapCache = new Dictionary<string, BitmapSource>();
		static readonly IResourceService ResourceService;
		
		static PresentationResourceService()
		{
			ResourceService = CompositionServices.Container.Resolve<IResourceService>();
			ResourceService.LanguageChanged += OnLanguageChanged;
		}
		
		static void OnLanguageChanged([NotNull] object sender, [NotNull] EventArgs e)
		{
		    lock (BitmapCache)
		    {
		        BitmapCache.Clear();
		    }
		}

		
		/// <summary>
		/// Returns a BitmapSource from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		[NotNull]
		public static BitmapSource GetBitmapSource([NotNull] string name)
		{
		    if (ResourceService == null) throw new ArgumentNullException("resourceService");
		    lock (BitmapCache)
		    {
		        BitmapSource bs;
		        if (BitmapCache.TryGetValue(name, out bs)) return bs;
		        var bmp = (System.Drawing.Bitmap) ResourceService.GetImageResource(name);
		        if (bmp == null) throw new ResourceNotFoundException(name);
		        IntPtr hBitmap = bmp.GetHbitmap();
		        try
		        {
		            bs = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
		                                                       Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		            bs.Freeze();
		            BitmapCache[name] = bs;
		        }
		        finally
		        {
		            NativeMethods.DeleteObject(hBitmap);
		        }
		        return bs;
		    }
		}
	}
}
