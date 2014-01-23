// The file ImageSourceHelper.cs is part of Tauron.Application.Common.Wpf.
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
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSourceHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Defines the ImageSourceHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Markup;
using Microsoft.Practices.EnterpriseLibrary.Logging;

#endregion

namespace Tauron.Application.Converter
{
    internal static class ImageSourceHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The enter.
        /// </summary>
        /// <param name="imageSource">
        ///     The image source.
        /// </param>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool Enter(string imageSource, IServiceProvider provider)
        {
            if (imageSource == null)
            {
                if (Logger.IsLoggingEnabled())
                {
                    Logger.Write(
                        new LogEntry
                        {
                            Severity = TraceEventType.Warning,
                            Message =
                                string.Format(
                                    "InmageSource are null. {0}",
                                    provider.GetService<IProvideValueTarget>().TargetObject)
                        });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     The exit.
        /// </summary>
        /// <param name="imageSource">
        ///     The image source.
        /// </param>
        /// <param name="isNull">
        ///     The is null.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool Exit(string imageSource, bool isNull)
        {
            if (isNull)
            {
                if (Logger.IsLoggingEnabled())
                {
                    Logger.Write(
                        new LogEntry
                        {
                            Severity = TraceEventType.Warning,
                            Message = string.Format("Inmage not Found: {0}.", imageSource)
                        });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     The resolve assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string ResolveAssembly(string assembly, IServiceProvider provider)
        {
            if (assembly == "this")
            {
                var target = provider.GetService<IProvideValueTarget>();
                return target == null ? assembly : target.TargetObject.GetType().Assembly.FullName;
            }

            return assembly;
        }

        #endregion
    }
}