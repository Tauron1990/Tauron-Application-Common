// The file CollectionExtensions.cs is part of Tauron.Application.Common.
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
// <copyright file="CollectionExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Stellt Erweiterung Methoden für Auflistungen zur Verfügung.</summary>
    [PublicAPI]
    public static class CollectionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fügt einen Wert hinzu wenn kein eintsprechender Schlüssel Vormanden ist und
        ///     gibt den Inhalt anschliesen zurück.
        /// </summary>
        /// <param name="dic">
        ///     Das Dicionary in das der Wert eingefügt werden soll.
        /// </param>
        /// <param name="key">
        ///     Der Schlussel der geprüft werden soll.
        /// </param>
        /// <param name="creator">
        ///     Eine Methode die den Inhalt bei bedarf erstellt.
        /// </param>
        /// <example>
        ///     Beispielhafte darstellung des Aufrufs <see cref="AddIfNotExis{TKey,TValue}" />
        ///     <code>
        /// var value = dic.(key, () =&gt; Creator());
        /// </code>
        /// </example>
        /// <typeparam name="TKey">
        ///     Der Type des Schlüssels.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Der Type des Inhalts.
        /// </typeparam>
        /// <returns>
        ///     Der Wert, der entwerder erstellt wurde oder schon enthalten war.
        /// </returns>
        public static TValue AddIfNotExis<TKey, TValue>(
            this IDictionary<TKey, TValue> dic,
            TKey key,
            Func<TValue> creator)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Requires<ArgumentNullException>(key != null, "key");
            Contract.Requires<ArgumentNullException>(creator != null, "creator");

            TValue temp;

            if (dic.ContainsKey(key)) temp = dic[key];
            else
            {
                temp = creator();
                dic[key] = temp;
            }

            return temp;
        }

        /// <summary>
        ///     Gibt den Inhalt eines Schlüssels von einem Dictionary&lt;string,object&gt; zurück und
        ///     Castet ihn in den richtigen Typen.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Gibt den Inhalt eines Schlüssels von einem Dictionary&lt;string,object&gt; zurück und
        ///         Castet ihn in den richtigen Typen.
        ///     </para>
        ///     <para>
        ///         Wenn kein Schlüssel vorhanden ist oder der Type Falsch ist wird null zurück gegeben.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     Beispielhafte Darstellung eines Aufrufes.
        ///     <code>
        /// IDictionary dic = Initialize();
        /// Type sample = dic.TryGetAndCast&lt;Type&gt;("SampleKey");
        /// </code>
        /// </example>
        /// <param name="dic">
        ///     Das
        ///     <seealso cref="IDictionary{Key, Value}" />
        ///     , dessen Wert entnommen werden soll.
        /// </param>
        /// <param name="key">
        ///     Der Schlüssel nach dem Gesucht werden soll.
        /// </param>
        /// <typeparam name="TValue">
        ///     Der Type in dem der Wert gecastet werden soll.
        /// </typeparam>
        /// <returns>
        ///     Der gecastete wert oder null.
        /// </returns>
        public static TValue TryGetAndCast<TValue>(this IDictionary<string, object> dic, string key)
            where TValue : class
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Requires<ArgumentNullException>(key != null, "key");

            object obj;
            if (dic.TryGetValue(key, out obj)) return obj as TValue;

            return null;
        }

        #endregion
    }
}