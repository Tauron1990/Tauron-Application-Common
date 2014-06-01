#region

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The new.</summary>
    [PublicAPI]
    public static class Factory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The object.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TObject">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TObject" />.
        /// </returns>
        [NotNull]
        public static TObject Object<TObject>([NotNull] params object[] args)
            where TObject : class
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Ensures(Contract.Result<TObject>() != null);

            var tracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(typeof (TObject), tracer, new BuildParameter[0], args);
            if(tracer.Exceptional)
                throw new BuildUpException(tracer);

            return (TObject) val;
        }

        [NotNull]
        public static object Object([NotNull] Type targetType, [NotNull] params object[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Ensures(Contract.Result<object>() != null);

            var errorTracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(targetType, errorTracer, new BuildParameter[0], args);
            if(errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);

            return val;
        }

        #endregion
    }
}