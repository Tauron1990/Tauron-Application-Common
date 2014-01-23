// The file SerializationExtensions.cs is part of Tauron.Application.Common.
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
// <copyright file="SerializationExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The serialization extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The serialization extensions.</summary>
    [PublicAPI]
    public static class SerializationExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue Deserialize<TValue>(this string path, IFormatter formatter)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Ensures(Contract.Result<TValue>() != null);

            using (FileStream stream = File.OpenRead(path)) return (TValue) InternalDeserialize(formatter, stream);
        }

        /// <summary>
        ///     The deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue Deserialize<TValue>(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Ensures(Contract.Result<TValue>() != null);

            if (!File.Exists(path)) return Activator.CreateInstance<TValue>();

            using (FileStream stream = File.OpenRead(path)) return (TValue) InternalDeserialize(new BinaryFormatter(), stream);
        }

        /// <summary>
        ///     The serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void Serialize(this object graph, IFormatter formatter, string path)
        {
            Contract.Requires<ArgumentNullException>(graph != null, "graph");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            using (FileStream stream = File.OpenWrite(path)) InternalSerialize(graph, formatter, stream);
        }

        /// <summary>
        ///     The serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void Serialize(this object graph, string path)
        {
            Contract.Requires<ArgumentNullException>(graph != null, "graph");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            path.CreateDirectoryIfNotExis();
            using (FileStream stream = File.OpenWrite(path)) InternalSerialize(graph, new BinaryFormatter(), stream);
        }

        /// <summary>
        ///     The xml deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue XmlDeserialize<TValue>(this string path, XmlSerializer formatter)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Ensures(Contract.Result<TValue>() != null);

            using (FileStream stream = File.OpenRead(path)) return (TValue) InternalDeserialize(new XmlSerilalizerDelegator(formatter), stream);
        }

        /// <summary>
        ///     The xml deserialize if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public static TValue XmlDeserializeIfExis<TValue>(this string path, XmlSerializer formatter)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Ensures(Contract.Result<TValue>() != null);

            return path.ExisFile() ? XmlDeserialize<TValue>(path, formatter) : Activator.CreateInstance<TValue>();
        }

        /// <summary>
        ///     The xml serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void XmlSerialize(this object graph, XmlSerializer formatter, string path)
        {
            Contract.Requires<ArgumentNullException>(graph != null, "graph");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            using (FileStream stream = File.OpenWrite(path)) InternalSerialize(graph, new XmlSerilalizerDelegator(formatter), stream);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The internal deserialize.
        /// </summary>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [ContractVerification(false)]
        private static object InternalDeserialize(IFormatter formatter, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Requires<ArgumentNullException>(stream != null, "stream");
            Contract.Ensures(Contract.Result<object>() != null);

            return formatter.Deserialize(stream);
        }

        /// <summary>
        ///     The internal serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        private static void InternalSerialize(object graph, IFormatter formatter, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(graph != null, "graph");
            Contract.Requires<ArgumentNullException>(formatter != null, "formatter");
            Contract.Requires<ArgumentNullException>(stream != null, "stream");

            formatter.Serialize(stream, graph);
        }

        #endregion

        /// <summary>The xml serilalizer delegator.</summary>
        private class XmlSerilalizerDelegator : IFormatter
        {
            #region Fields

            /// <summary>The _serializer.</summary>
            private readonly XmlSerializer serializer;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="XmlSerilalizerDelegator" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="XmlSerilalizerDelegator" /> Klasse.
            ///     Initializes a new instance of the <see cref="XmlSerilalizerDelegator" /> class.
            /// </summary>
            /// <param name="serializer">
            ///     The serializer.
            /// </param>
            public XmlSerilalizerDelegator(XmlSerializer serializer)
            {
                Contract.Requires<ArgumentNullException>(serializer != null, "serializer");

                this.serializer = serializer;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the binder.</summary>
            /// <value>The binder.</value>
            public SerializationBinder Binder
            {
                get { return null; }

                set { }
            }

            /// <summary>Gets or sets the context.</summary>
            /// <value>The context.</value>
            public StreamingContext Context
            {
                get { return new StreamingContext(); }

                set { }
            }

            /// <summary>Gets or sets the surrogate selector.</summary>
            /// <value>The surrogate selector.</value>
            public ISurrogateSelector SurrogateSelector
            {
                get { return null; }

                set { }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The deserialize.
            /// </summary>
            /// <param name="serializationStream">
            ///     The serialization stream.
            /// </param>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            public object Deserialize(Stream serializationStream)
            {
                return serializer.Deserialize(serializationStream);
            }

            /// <summary>
            ///     The serialize.
            /// </summary>
            /// <param name="serializationStream">
            ///     The serialization stream.
            /// </param>
            /// <param name="graph">
            ///     The graph.
            /// </param>
            public void Serialize(Stream serializationStream, object graph)
            {
                serializer.Serialize(serializationStream, graph);
            }

            #endregion

            #region Methods

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(serializer != null);
            }

            #endregion
        }
    }
}