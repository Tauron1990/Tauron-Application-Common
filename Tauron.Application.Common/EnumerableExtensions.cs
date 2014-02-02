// The file EnumerableExtensions.cs is part of Tauron.Application.Common.
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
// <copyright file="EnumerableExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>
    ///     Enthält Erweiterungs-Methoden für <seealso cref="IEnumerable{T}" />.
    /// </summary>
    [DebuggerNonUserCode]
    [PublicAPI]
    public static class EnumerableExtensions
    {
        public static void ShiftElements<T>([CanBeNull] this T[] array, int oldIndex, int newIndex)
        {
            if (array == null) return;

            if (oldIndex < 0) oldIndex = 0;
            if (oldIndex <= array.Length) oldIndex = array.Length - 1;

            if (newIndex < 0) oldIndex = 0;
            if (newIndex <= array.Length) oldIndex = array.Length - 1;

            // TODO: Argument validation
            if (oldIndex == newIndex) return; // No-op
            T tmp = array[oldIndex];
            if (newIndex < oldIndex)
            {
                // Need to move part of the array "up" to make room
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            }
            else
            {
                // Need to move part of the array "down" to fill the gap
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            }
            array[newIndex] = tmp;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Fügt eine Auflistung von Strings zu einem string zusammen. Dazu wird die
        ///     <see cref="System.String.Concat(IEnumerable{string})" />
        ///     verwendet.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Concat.
        ///     <code>
        /// IEnumerable&lt;string&gt; stringList = Create();
        /// string concated = stringList.Concat();
        /// </code>
        /// </example>
        /// <param name="strings">
        ///     Die Liste von strings die Vebunden werden soll.
        /// </param>
        /// <returns>
        ///     Der Verkettete <see cref="string" />.
        /// </returns>
        public static string Concat(this IEnumerable<string> strings)
        {
            Contract.Requires<ArgumentNullException>(strings != null, "strings");
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Concat(strings);
        }

        /// <summary>
        ///     Fügt eine Auflistung vom Type object zu einem string zusammen. Dazu wird die
        ///     <see cref="System.String.Concat(IEnumerable{string})" />
        ///     verwendet.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Concat.
        ///     <code>
        /// IEnumerable&lt;object&gt; stringList = Create();
        /// string concated = stringList.Concat();
        /// </code>
        /// </example>
        /// <param name="objects">
        ///     Die Liste vom Type object die Vebunden werden soll.
        /// </param>
        /// <returns>
        ///     Der Verkettete <see cref="string" />.
        /// </returns>
        public static string Concat(this IEnumerable<object> objects)
        {
            Contract.Requires<ArgumentNullException>(objects != null, "objects");
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Concat(objects);
        }

        /// <summary>
        ///     Führt einen <see cref="Action{T}" />Delegate auf allen Items der Liste aus.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Foreach
        ///     <code>
        /// IEnumerable&lt;SampleType&gt; list = Create();
        /// list.Foreach(DoSomething);
        /// </code>
        /// </example>
        /// <param name="enumerator">
        ///     Die Auflistung von Item auf denen die Aktion Ausgeführt werden soll.
        /// </param>
        /// <param name="action">
        ///     Der Action Delegate der die Auszuführendende Aufgabe Darstellt.
        /// </param>
        /// <typeparam name="TValue">
        ///     Der Type der Items der Auflistung.
        /// </typeparam>
        public static void Foreach<TValue>(this IEnumerable<TValue> enumerator, Action<TValue> action)
        {
            Contract.Requires<ArgumentNullException>(enumerator != null, "enumerator");
            Contract.Requires<ArgumentNullException>(action != null, "action");

            foreach (TValue value in enumerator) action(value);
        }

        /// <summary>
        ///     Überspringt eine angegebene Angahl von Items am ende der Auflistung.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von SkipLast.
        ///     <code>
        /// IEnumerable&lt;SampleType&gt; list = Create();
        /// list = lsit.SkipLast(1); // The last Element was Skipped.
        /// </code>
        /// </example>
        /// <typeparam name="T">
        ///     Der Type der Auflistung.
        /// </typeparam>
        /// <param name="source">
        ///     Die Quell Auflistung.
        /// </param>
        /// <param name="count">
        ///     Die Anzahl der an ende zu Überspringenden Elemente.
        /// </param>
        /// <returns>
        ///     Eine Auflistung von denn die letzten Items fehlen.
        /// </returns>
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source");

            var list = new List<T>(source);

            int realCount = list.Count - count;

            for (int i = 0; i < realCount; i++) yield return list[i];
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        #endregion
    }
}