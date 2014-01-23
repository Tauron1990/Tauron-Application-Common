﻿// The file EditableValueMap.cs is part of Tauron.Application.Common.
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
// <copyright file="EditableValueMap.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Eine Statische Klasse die Instanzen vom Type EditableValueMap erstellt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Eine Statische Klasse die Instanzen vom Type EditableValueMap erstellt.</summary>
    [PublicAPI]
    public static class EditableValueMap
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Erstellt eine Instanz vom Type EditableValueMap&lt;T1, T2&gt;.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf der methode Create.
        ///     <code>
        /// var map = EditableValueMap.Create(Type2 value1, Type2 value2);
        /// </code>
        /// </example>
        /// <param name="item1">
        ///     Der Wert des Ersten Items.
        /// </param>
        /// <param name="item2">
        ///     Der Wert des zweiten items.
        /// </param>
        /// <typeparam name="T1">
        ///     Der Type des ersten Items.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     Der Type des zweiten Items.
        /// </typeparam>
        /// <returns>
        ///     Eine instanz vom Type <see cref="EditableValueMap{T1, T2}" />.
        /// </returns>
        public static EditableValueMap<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            Contract.Ensures(Contract.Result<EditableValueMap<T1, T2>>() != null);

            return new EditableValueMap<T1, T2> {Item1 = item1, Item2 = item2};
        }

        #endregion
    }

    /// <summary>
    ///     Stellt eine Klasse dar die zwei Typisierte und beabeitbare Werte in einer Klasse bündelt.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Stellt eine Klasse dar die zwei Typisierte und beabeitbare Werte in einer Klasse bündelt.
    ///     </para>
    ///     <para>
    ///         Implementiert das <see cref="IWeakReference" /> Interface und überprüft beide Items ob sie als
    ///         <see cref="System.WeakReference" />
    ///         oder <see cref="IWeakReference" /> gecastet werden können. Ist das nicht der Fall wird immer true
    ///         zurückgegeben.
    ///     </para>
    /// </remarks>
    /// <typeparam name="T1">
    ///     Der Type des ersten Items.
    /// </typeparam>
    /// <typeparam name="T2">
    ///     Der Type des zweiten Items.
    /// </typeparam>
    [PublicAPI]
    public sealed class EditableValueMap<T1, T2> : IWeakReference
    {
        #region Public Properties

        /// <summary>Gibt den wert des item 1 zurück oder legt ihn fest.</summary>
        /// <value>Der Typesierter erste Wert.</value>
        public T1 Item1 { get; set; }

        /// <summary>Gibt den wert des item 1 zurück oder legt ihn fest.</summary>
        /// <value>Der Typesierter zeiter Wert.</value>
        public T2 Item2 { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>Wenn eines der beiden Item einen Schwacher Verweis Ist gibt diese Eigenscht dessen Wert zurück.</summary>
        /// <value>Ob diese Instnz noch verfügbar ist.</value>
        bool IWeakReference.IsAlive
        {
            get
            {
                IWeakReference reference = Item1 as IWeakReference ?? Item2 as IWeakReference;
                if (reference != null) return reference.IsAlive;

                WeakReference reference2 = Item1 as WeakReference ?? Item2 as WeakReference;
                return reference2 == null || reference2.IsAlive;
            }
        }

        #endregion
    }
}