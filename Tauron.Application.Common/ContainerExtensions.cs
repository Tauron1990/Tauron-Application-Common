#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The container extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Stellt Erweiterung Methoden für den Ioc Container zur Verfügung.</summary>
    /// <remarks>
    ///     <para>Die Konkreteren methoden zum Abrufen von Instanzen werden mit Erweiterung Methoden Implementiert.</para>
    ///     <para>Dadurch bleibt das Container Interface Sauber.</para>
    /// </remarks>
    [PublicAPI]
    public static class ContainerExtensions
    {
        #region Constants

        private const string ErrorMessage = "Error on Return of Container Resolve";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Ruft einene Instanz aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve&lt;Service&gt;();
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <typeparam name="TType">
        ///     Der Type der Abgerufen werden soll.
        /// </typeparam>
        /// <returns>
        ///     Eine Instanz von <see cref="TType" />.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        [NotNull]
        public static TType Resolve<TType>([NotNull] this IContainer con) where TType : class
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");

            Contract.Ensures(Contract.Result<TType>() != null, ErrorMessage);

            return (TType) con.Resolve(typeof (TType), null);
        }

        /// <summary>
        ///     The resolve.
        /// </summary>
        /// <param name="con">
        ///     The con.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="optional">
        ///     The optional.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType Resolve<TType>(this IContainer con, string name, bool optional, params BuildParameter[] buildParameters) where TType : class
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");

            Contract.Ensures(optional || Contract.Result<TType>() != null, ErrorMessage);

            return con.Resolve(typeof (TType), name, optional, buildParameters) as TType;
        }

        /// <summary>
        ///     Ruft einene Instanz mit Vertragsnamen aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve(typeof(Service), "VertragsName");
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <returns>
        ///     Eine Instanz des Exports.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static object Resolve(this IContainer con, Type @interface, string name, params BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");
            Contract.Requires<ArgumentNullException>(@interface != null, "interface");

            Contract.Ensures(Contract.Result<object>() != null, ErrorMessage);

            var tracer = new ErrorTracer();

            try
            {
                var expo = con.FindExport(@interface, name, tracer);
                return tracer.Exceptional ? null : con.BuildUp(expo, tracer, buildParameters);
            }
            finally
            {
                if(tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     Ruft einene Instanz mit Vertragsnamen aus dem Build System des Containers ab
        ///     und gibt null zurück wenn kein Export gefunden wurde.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve(typeof(Service), "VertragsName", true);
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <param name="optional">
        ///     Gibt an ob der Export Optional ist oder nicht.
        /// </param>
        /// <returns>
        ///     Eine Instanz des Exports.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static object Resolve(this IContainer con, Type @interface, string name, bool optional, BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");
            Contract.Requires<ArgumentNullException>(@interface != null, "interface");

            var tracer = new ErrorTracer();

            try
            {
                ExportMetadata data = con.FindExport(@interface, name, tracer, optional);

                if (tracer.Exceptional) return null;
                if (data != null) return con.BuildUp(data, tracer, buildParameters);

                if (optional) return null;
                return null;
            }
            finally
            {
                if(tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     Ruft alle Instanzen aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// IEnumerable&lt;object&gt; instanz = container.ResolveAll(typeof(Service), "VertragsName");
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <returns>
        ///     Eine Auflistung von Instanzen des Exports als <see cref="IEnumerable{TType}" />.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static IEnumerable<object> ResolveAll(this IContainer con, Type @interface, string name, params BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");
            Contract.Requires<ArgumentNullException>(@interface != null, "interface");

            Contract.Ensures(Contract.Result<IEnumerable<object>>() != null, ErrorMessage);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<object>>(), obj => obj != null), ErrorMessage);

            var tracer = new ErrorTracer();

            try
            {
                var temp = con.FindExports(@interface, name, tracer);
                if(tracer.Exceptional) yield break;

                foreach (var exportMetadata in temp)
                {
                    var tempBuild = con.BuildUp(exportMetadata, tracer, buildParameters);
                    if(tracer.Exceptional) yield break;

                    yield return tempBuild;
                }
            }
            finally
            {
                if(tracer.Exceptional)
                    throw  new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     The resolve all.
        /// </summary>
        /// <param name="con">
        ///     The con.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<TType> ResolveAll<TType>(this IContainer con, string name)
        {
            Contract.Requires<ArgumentNullException>(con != null, "con");

            Contract.Ensures(Contract.Result<IEnumerable<TType>>() != null, ErrorMessage);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<TType>>(), obj => obj != null), ErrorMessage);

            return ResolveAll(con, typeof(TType), name).Cast<TType>();
        }

        #endregion
    }
}