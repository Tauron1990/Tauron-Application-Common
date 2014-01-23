﻿// The file WeakCollection.cs is part of Tauron.Application.Common.
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
// <copyright file="WeakCollection.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The weak collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The weak collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WeakCollection<TType> : IList<TType>
        where TType : class
    {
        #region Fields

        /// <summary>The _internal collection.</summary>
        private readonly List<WeakReference<TType>> _internalCollection = new List<WeakReference<TType>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakCollection{TType}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakCollection{TType}" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakCollection{TType}" /> class.
        /// </summary>
        public WeakCollection()
        {
            WeakCleanUp.RegisterAction(CleanUp);
        }

        #endregion

        #region Public Events

        /// <summary>The cleaned event.</summary>
        public event EventHandler CleanedEvent;

        #endregion

        #region Public Properties

        /// <summary>Gets the effective count.</summary>
        /// <value>The effective count.</value>
        public int EffectiveCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _internalCollection.Count(refer => refer.IsAlive());
            }
        }

        /// <summary>Gets the count.</summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _internalCollection.Count;
            }
        }

        /// <summary>Gets a value indicating whether is read only.</summary>
        /// <value>The is read only.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     The this.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The Value.
        /// </returns>
        public TType this[int index]
        {
            get { return _internalCollection[index].TypedTarget(); }

            set { _internalCollection[index] = new WeakReference<TType>(value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        [ContractVerification(false)]
        public void Add(TType item)
        {
            if (item == null) return;

            _internalCollection.Add(new WeakReference<TType>(item));
        }

        /// <summary>The clear.</summary>
        public void Clear()
        {
            _internalCollection.Clear();
        }

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [ContractVerification(false)]
        public bool Contains(TType item)
        {
            if (item == null) return false;

            return _internalCollection.Any(it => it.TypedTarget() == item);
        }

        /// <summary>
        ///     The copy to.
        /// </summary>
        /// <param name="array">
        ///     The array.
        /// </param>
        /// <param name="arrayIndex">
        ///     The array index.
        /// </param>
        public void CopyTo(TType[] array, int arrayIndex)
        {
            int index = 0;
            for (int i = arrayIndex; i < array.Length; i++)
            {
                TType target = null;
                while (target == null && index <= _internalCollection.Count)
                {
                    target = _internalCollection[index].TypedTarget();
                    index++;
                }

                if (target == null) break;

                array[i] = target;
            }
        }

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<TType> GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator<TType>>() != null);

            return
                _internalCollection.Select(reference => reference.TypedTarget())
                                   .Where(target => target != null)
                                   .GetEnumerator();
        }

        /// <summary>
        ///     The index of.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int IndexOf(TType item)
        {
            if (item == null) return -1;

            int index;
            for (index = 0; index < _internalCollection.Count; index++)
            {
                WeakReference<TType> temp = _internalCollection[index];
                if (temp.TypedTarget() == item) break;
            }

            return index == _internalCollection.Count ? -1 : index;
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        public void Insert(int index, TType item)
        {
            if (item == null) return;

            _internalCollection.Insert(index, new WeakReference<TType>(item));
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Remove(TType item)
        {
            if (item == null) return false;

            int index = IndexOf(item);
            if (index == -1) return false;

            _internalCollection.RemoveAt(index);
            return true;
        }

        /// <summary>
        ///     The remove at.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        [ContractVerification(false)]
        public void RemoveAt(int index)
        {
            _internalCollection.RemoveAt(index);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator>() != null);

            return GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>The clean up.</summary>
        internal void CleanUp()
        {
            WeakReference<TType>[] dead = _internalCollection.Where(reference => !reference.IsAlive()).ToArray();

            foreach (var genericWeakReference in dead) _internalCollection.Remove(genericWeakReference);

            OnCleaned();
        }

        /// <summary>The on cleaned.</summary>
        private void OnCleaned()
        {
            if (CleanedEvent != null) CleanedEvent(this, EventArgs.Empty);
        }

        #endregion
    }

    /// <summary>
    ///     The weak reference collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode]
    public class WeakReferenceCollection<TType> : Collection<TType>
        where TType : IWeakReference
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakReferenceCollection{TType}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakReferenceCollection{TType}" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakReferenceCollection{TType}" /> class.
        /// </summary>
        public WeakReferenceCollection()
        {
            WeakCleanUp.RegisterAction(CleanUpMethod);
        }

        #endregion

        #region Methods

        /// <summary>The clear items.</summary>
        protected override void ClearItems()
        {
            lock (this)
            {
                base.ClearItems();
            }
        }

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, TType item)
        {
            lock (this)
            {
                if (index > Count) index = Count;

                base.InsertItem(index, item);
            }
        }

        /// <summary>
        ///     The remove item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            lock (this) base.RemoveItem(index);
        }

        /// <summary>
        ///     The set item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void SetItem(int index, TType item)
        {
            lock (this) base.SetItem(index, item);
        }

        /// <summary>The clean up method.</summary>
        private void CleanUpMethod()
        {
            lock (this)
                Items.ToArray().Where(it => !it.IsAlive).ToArray().Foreach(
                    it =>
                    {
                        var dis = it as IDisposable;
                        if (dis != null) dis.Dispose();

                        Items.Remove(it);
                    });
        }

        #endregion
    }
}