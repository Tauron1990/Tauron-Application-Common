// The file GroupDirectory.cs is part of Tauron.Application.Common.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     Eine Klasse die eine Liste von Objecten einem Schlüssel zuordnent.
    /// </summary>
    /// <typeparam name="TKey">
    ///     Der Type des Schlüssels.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     Der Type des Inhalts.
    /// </typeparam>
    [Serializable]
    [PublicAPI]
    public class GroupDictionary<TKey, TValue> : Dictionary<TKey, ICollection<TValue>>
        where TKey : class where TValue : class
    {
        #region Serializable

        /// <summary>
        ///     Implementiert die <see cref="T:System.Runtime.Serialization.ISerializable" />-Schnittstelle und gibt die zum
        ///     Serialisieren der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz erforderlichen Daten zurück.
        /// </summary>
        /// <param name="info">
        ///     Ein <see cref="T:System.Runtime.Serialization.SerializationInfo" />-Objekt mit den zum Serialisieren der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz erforderlichen Informationen.
        /// </param>
        /// <param name="context">
        ///     Eine <see cref="T:System.Runtime.Serialization.StreamingContext" />-Struktur, die die Quelle und das Ziel des
        ///     serialisierten Streams enthält, der der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz zugeordnet ist.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="info" /> ist null.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("listType", _listType, typeof (Type));

            base.GetObjectData(info, context);
        }

#pragma warning disable 628

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1047:DoNotDeclareProtectedMembersInSealedTypes")]
        protected GroupDictionary(SerializationInfo info, StreamingContext context)
#pragma warning restore 628
            : base(info, context)
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");

            _listType = (Type) info.GetValue("listType", typeof (Type));
        }

        #endregion Serializable

        /// <summary>The _list type.</summary>
        private readonly Type _listType;

        /// <summary>The _generic temp.</summary>
        private Type _genericTemp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="listType">
        ///     The list type.
        /// </param>
        public GroupDictionary(Type listType)
        {
            Contract.Requires<ArgumentNullException>(listType != null, "listType");

            _listType = listType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        public GroupDictionary()
        {
            _listType = typeof (List<TValue>);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="singleList">
        ///     The single list.
        /// </param>
        public GroupDictionary(bool singleList)
        {
            _listType = singleList ? typeof (HashSet<TValue>) : typeof (List<TValue>);
        }

        /// <summary>Gibt eine Collection zurück die Alle in den Listen enthaltenen Werte Darstellen.</summary>
        /// <value>The all values.</value>
        public ICollection<TValue> AllValues
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<TValue>>() != null);

                return new AllValueCollection(this);
            }
        }

        /// <summary>
        ///     Gibt eine Liste mit entsprechenden Schlüssel zurück. Ist keine Liste bkannt
        ///     wird eine erstellt.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel nach dem gesucht werden soll.
        /// </param>
        /// <returns>
        ///     Eine Passende Collection.
        /// </returns>
        [ContractVerification(false)]
        public new ICollection<TValue> this[TKey key]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(key != null, "TKey");
                Contract.Ensures(Contract.Result<ICollection<TValue>>() != null);

                if (!ContainsKey(key)) Add(key);

                return base[key];
            }

            set
            {
                Contract.Requires<ArgumentNullException>(key != null, "key");
                Contract.Requires<ArgumentNullException>(value != null, "value");

                base[key] = value;
            }
        }

        /// <summary>The create list.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        private object CreateList()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            if (!typeof (ICollection<TValue>).IsAssignableFrom(_listType)) throw new InvalidOperationException();

            if (_genericTemp != null) return Activator.CreateInstance(_genericTemp);

            if (_listType.ContainsGenericParameters)
            {
                if (_listType.GetGenericArguments().Length != 1) throw new InvalidOperationException();

                _genericTemp = _listType.MakeGenericType(typeof (TValue));
            }
            else
            {
                Type[] generic = _listType.GetGenericArguments();
                if (generic.Length > 1) throw new InvalidOperationException();

                if (generic.Length == 0) _genericTemp = _listType;

                if (_genericTemp == null && generic[0] == typeof (TValue)) _genericTemp = _listType;
                else _genericTemp = _listType.GetGenericTypeDefinition().MakeGenericType(typeof (TValue));
            }

            if (_genericTemp == null) throw new InvalidOperationException();

            return Activator.CreateInstance(_genericTemp);
        }

        /// <summary>
        ///     Fügt einen schlüssel zur liste hinzu.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel der hinzugefügt werden soll.
        /// </param>
        public void Add(TKey key)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");

            if (!ContainsKey(key)) base[key] = (ICollection<TValue>) CreateList();
        }

        /// <summary>
        ///     Fügt eineesn schlüssel und ein Element hinzu bei .
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel unter dem ein wert hinzugefügt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Hinzugefügt werden soll.
        /// </param>
        public void Add(TKey key, TValue value)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");
            Contract.Requires<ArgumentNullException>(value != null, "value");

            if (!ContainsKey(key)) Add(key);

            ICollection<TValue> list = base[key];
            if (list != null) list.Add(value);
        }

        /// <summary>
        ///     Fügt eine ganze liste von werten hinzu.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel zu dem ein wert hinzugefügt werden soll.
        /// </param>
        /// <param name="value">
        ///     Die werte die hinzugefügt werden sollen.
        /// </param>
        public void AddRange(TKey key, IEnumerable<TValue> value)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");
            Contract.Requires<ArgumentNullException>(value != null, "value");

            if (!ContainsKey(key)) Add(key);

            ICollection<TValue> values = base[key];
            if (values != null) foreach (TValue item in value.Where(item => item != null)) values.Add(item);
        }

        /// <summary>
        ///     Entfernt einen wert unabhänig vom schlüssel.
        /// </summary>
        /// <param name="value">
        ///     Der wert der entfernt werden soll.
        /// </param>
        /// <returns>
        ///     Ob der wert Entfernt werden konnte.
        /// </returns>
        public bool RemoveValue(TValue value)
        {
            return RemoveImpl(null, value, false, true);
        }

        /// <summary>
        ///     Entfernt einen wert unabhänig vom schlüssel.
        /// </summary>
        /// <param name="value">
        ///     Der wert der entfernt werden soll.
        /// </param>
        /// <param name="removeEmptyLists">
        ///     Gibt an ob leere listen entfernt werden sollen.
        /// </param>
        /// <returns>
        ///     Ob der wert Entfernt werden konnte.
        /// </returns>
        public bool Remove(TValue value, bool removeEmptyLists)
        {
            return RemoveImpl(null, value, removeEmptyLists, true);
        }

        /// <summary>
        ///     Entfernt einen Wert aus der Liste eines bestimten schlüssels.
        /// </summary>
        /// <param name="key">
        ///     Der schlüssel der den Wert enthält der Enfernt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Enfernt werden soll.
        /// </param>
        /// <returns>
        ///     Ob der wert Enfernt werden konnte.
        /// </returns>
        public bool Remove(TKey key, TValue value)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");

            return RemoveImpl(key, value, false, false);
        }

        /// <summary>
        ///     Entfernt einen Wert aus der Liste eines bestimten schlüssels.
        /// </summary>
        /// <param name="key">
        ///     Der schlüssel der den Wert enthält der Enfernt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Enfernt werden soll.
        /// </param>
        /// <param name="removeListIfEmpty">
        ///     Gibt an ob alle leeren listen entfernt werden sollen.
        /// </param>
        /// <returns>
        ///     Ob der wert Enfernt werden konnte.
        /// </returns>
        public bool Remove(TKey key, TValue value, bool removeListIfEmpty)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");

            return RemoveImpl(key, value, removeListIfEmpty, false);
        }

        /// <summary>
        ///     The remove impl.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <param name="removeEmpty">
        ///     The remove empty.
        /// </param>
        /// <param name="removeAll">
        ///     The remove all.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool RemoveImpl(TKey key, TValue val, bool removeEmpty, bool removeAll)
        {
            bool ok = false;

            if (removeAll)
            {
                IEnumerator keys = Keys.ToArray().GetEnumerator();
                IEnumerator vals = Values.ToArray().GetEnumerator();
                while (keys.MoveNext() && vals.MoveNext())
                {
                    var coll = (ICollection<TValue>) vals.Current;
                    var currkey = (TKey) keys.Current;
                    ok |= RemoveList(coll, val);
                    if (removeEmpty && coll.Count == 0) ok |= Remove(currkey);
                }
            }	

                #region Single

            else
            {
                ok = ContainsKey(key);
                if (!ok) return ok;
                ICollection<TValue> col = base[key];
                Contract.Assume(col != null);
                ok |= RemoveList(col, val);
                if (!removeEmpty) return ok;
                if (col.Count == 0) ok |= Remove(key);
            }

            #endregion Single

            return ok;
        }

        /// <summary>
        ///     The remove list.
        /// </summary>
        /// <param name="vals">
        ///     The vals.
        /// </param>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool RemoveList(ICollection<TValue> vals, TValue val)
        {
            Contract.Requires<ArgumentNullException>(vals != null, "vals");

            bool ok = false;
            while (vals.Remove(val)) ok = true;

            return ok;
        }

        #region Nested type: AllValueCollection

        /// <summary>The all value collection.</summary>
        private class AllValueCollection : ICollection<TValue>
        {
            #region Fields

            /// <summary>The _list.</summary>
            private readonly GroupDictionary<TKey, TValue> _list;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="AllValueCollection" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="AllValueCollection" /> Klasse.
            ///     Initializes a new instance of the <see cref="AllValueCollection" /> class.
            /// </summary>
            /// <param name="list">
            ///     The list.
            /// </param>
            public AllValueCollection(GroupDictionary<TKey, TValue> list)
            {
                Contract.Requires<ArgumentNullException>(list != null, "list");

                _list = list;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Ruft die Anzahl der Elemente ab, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten sind.
            /// </summary>
            /// <returns>
            ///     Die Anzahl der Elemente, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten sind.
            /// </returns>
            /// <value>The count.</value>
            public int Count
            {
                get { return GetAll.Count(); }
            }

            /// <summary>
            ///     Ruft einen Wert ab, der angibt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> schreibgeschützt ist.
            /// </summary>
            /// <returns>
            ///     True, wenn <see cref="T:System.Collections.Generic.ICollection`1" /> schreibgeschützt ist, andernfalls false.
            /// </returns>
            /// <value>The is read only.</value>
            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion

            #region Properties

            /// <summary>Gets the get all.</summary>
            /// <value>The get all.</value>
            private IEnumerable<TValue> GetAll
            {
                get
                {
                    Contract.Requires(_list != null);
                    Contract.Ensures(Contract.Result<IEnumerable<TValue>>() != null);

                    return _list.SelectMany(pair => pair.Value);
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Fügt der <see cref="T:System.Collections.Generic.ICollection`1" /> ein Element hinzu.
            /// </summary>
            /// <param name="item">
            ///     Das Objekt, das <see cref="T:System.Collections.Generic.ICollection`1" /> hinzugefügt werden soll.
            /// </param>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public void Add(TValue item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     Entfernt alle Elemente aus <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     Bestimmt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> einen bestimmten Wert enthält.
            /// </summary>
            /// <returns>
            ///     True, wenn sich <paramref name="item" /> in <see cref="T:System.Collections.Generic.ICollection`1" /> befindet,
            ///     andernfalls false.
            /// </returns>
            /// <param name="item">
            ///     Das im <see cref="T:System.Collections.Generic.ICollection`1" /> zu suchende Objekt.
            /// </param>
            [ContractVerification(false)]
            public bool Contains(TValue item)
            {
                return GetAll.Contains(item);
            }

            /// <summary>
            ///     Kopiert die Elemente von <see cref="T:System.Collections.Generic.ICollection`1" /> in ein
            ///     <see cref="T:System.Array" />
            ///     , beginnend bei einem bestimmten <see cref="T:System.Array" />-Index.
            /// </summary>
            /// <param name="array">
            ///     Das eindimensionale <see cref="T:System.Array" />, das das Ziel der aus
            ///     <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     kopierten Elemente ist.Für <see cref="T:System.Array" /> muss eine nullbasierte Indizierung verwendet werden.
            /// </param>
            /// <param name="arrayIndex">
            ///     Der nullbasierte Index in <paramref name="array" />, an dem das Kopieren beginnt.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="array" /> hat den Wert null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="arrayIndex" /> ist kleiner als 0.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     <paramref name="array" /> ist mehrdimensional.
            ///     - oder -Die Anzahl der Elemente in der Quelle <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     ist größer als der verfügbare Speicherplatz ab <paramref name="arrayIndex" /> bis zum Ende des
            ///     <paramref name="array" />
            ///     ,
            ///     das als Ziel festgelegt wurde.- oder -Type TValue kann nicht automatisch in den Typ des Ziel-
            ///     <paramref name="array" />
            ///     umgewandelt werden.
            /// </exception>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                GetAll.ToArray().CopyTo(array, arrayIndex);
            }

            /// <summary>Gibt einen Enumerator zurück, der die Auflistung durchläuft.</summary>
            /// <returns>
            ///     Ein <see cref="T:System.Collections.Generic.IEnumerator`1" />, der zum Durchlaufen der Auflistung verwendet werden
            ///     kann.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<TValue> GetEnumerator()
            {
                Contract.Ensures(Contract.Result<IEnumerator<TValue>>() != null);

                return GetAll.GetEnumerator();
            }

            /// <summary>
            ///     Entfernt das erste Vorkommen eines bestimmten Objekts aus <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     .
            /// </summary>
            /// <returns>
            ///     True, wenn <paramref name="item" /> erfolgreich aus <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     gelöscht wurde, andernfalls false.Diese Methode gibt auch dann false zurück, wenn
            ///     <paramref name="item" />
            ///     nicht in der ursprünglichen <see cref="T:System.Collections.Generic.ICollection`1" /> gefunden wurde.
            /// </returns>
            /// <param name="item">
            ///     Das aus dem <see cref="T:System.Collections.Generic.ICollection`1" /> zu entfernende Objekt.
            /// </param>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region Explicit Interface Methods

            /// <summary>Gibt einen Enumerator zurück, der eine Auflistung durchläuft.</summary>
            /// <returns>
            ///     Ein <see cref="T:System.Collections.IEnumerator" />-Objekt, das zum Durchlaufen der Auflistung verwendet werden
            ///     kann.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                Contract.Ensures(Contract.Result<IEnumerator>() != null);

                return GetEnumerator();
            }

            #endregion
        }

        #endregion Nested type: AllValueCollection
    }
}