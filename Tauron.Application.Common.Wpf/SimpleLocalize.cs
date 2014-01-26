// The file SimpleLocalize.cs is part of Tauron.Application.Common.Wpf.
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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Resources;
using System.Windows.Markup;
using System.Xaml;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The simple localize.</summary>
    [PublicAPI]
    public class SimpleLocalize : MarkupExtension
    {
        #region Static Fields

        private static readonly Dictionary<Assembly, ResourceManager> Resources =
            new Dictionary<Assembly, ResourceManager>();

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the name.</summary>
        [NotNull]
        public string Name { get; set; }

        #endregion

        public SimpleLocalize([NotNull] string name)
        {
            Name = name;
        }

        public SimpleLocalize()
        {
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="manager">
        ///     The manager.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        public static void Register([NotNull] ResourceManager manager, [NotNull] Assembly key)
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");
            Contract.Requires<ArgumentNullException>(key != null, "key");

            Resources[key] = manager;
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        public static void Remove([NotNull] Assembly key)
        {
            Resources.Remove(key);
        }

        /// <summary>
        ///     The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof (IRootObjectProvider)) as IRootObjectProvider;

            if (provider == null || provider.RootObject == null) return "null"; // "IRootObjectProvider oder das RootObject existieren nicht!";

            ResourceManager manager;

            return Resources.TryGetValue(provider.RootObject.GetType().Assembly, out manager)
                       ? manager.GetObject(Name)
                       : "null";
        }

        #endregion
    }
}