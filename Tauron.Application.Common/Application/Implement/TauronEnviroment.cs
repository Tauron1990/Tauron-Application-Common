// The file TauronEnviroment.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TauronEnviroment.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The tauron enviroment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Tauron.Application.Ioc;
using Tauron.Interop;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    // [DebuggerNonUserCode]

    /// <summary>The tauron enviroment.</summary>
    [PublicAPI]
    [Export(typeof (ITauronEnviroment))]
    public class TauronEnviroment : ITauronEnviroment
    {
        #region Constants

        /// <summary>The app repository.</summary>
        public const string AppRepository = "Tauron";

        #endregion

        #region Fields

        /// <summary>The _default path.</summary>
        private string _defaultPath;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the default profile path.</summary>
        /// <value>The default profile path.</value>
        public string DefaultProfilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultPath)) _defaultPath = LocalApplicationData;

                _defaultPath.CreateDirectoryIfNotExis();

                return _defaultPath;
            }

            set { _defaultPath = value; }
        }

        /// <summary>Gets the local application data.</summary>
        /// <value>The local application data.</value>
        public string LocalApplicationData
        {
            get
            {
                return
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath(AppRepository);
            }
        }

        /// <summary>Gets the local application temp folder.</summary>
        /// <value>The local application temp folder.</value>
        public string LocalApplicationTempFolder
        {
            get { return LocalApplicationData.CombinePath("Temp"); }
        }

        /// <summary>Gets the local download folder.</summary>
        /// <value>The local download folder.</value>
        public string LocalDownloadFolder
        {
            get { return SearchForFolder(KnownFolder.Downloads); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get profiles.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<string> GetProfiles(string application)
        {
            return
                DefaultProfilePath.CombinePath(application)
                                  .EnumerateDirectorys()
                                  .Select(ent => ent.Split('\\').Last());
        }

        /// <summary>
        ///     The search for folder.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string SearchForFolder(Guid id)
        {
            IntPtr pPath;
            if (NativeMethods.SHGetKnownFolderPath(id, 0, IntPtr.Zero, out pPath) == 0)
            {
                string s = Marshal.PtrToStringUni(pPath);
                Marshal.FreeCoTaskMem(pPath);
                return s;
            }

            return null;
        }

        #endregion
    }
}