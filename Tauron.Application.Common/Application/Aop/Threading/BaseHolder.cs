#region

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The BaseHolder interface.</summary>
    [PublicAPI]
    public interface IBaseHolder
    {
        #region Public Properties

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [NotNull]
        string Name { get; set; }

        #endregion
    }

    /// <summary>The base holder.</summary>
    public static class BaseHolder
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get or add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue GetOrAdd<TKey, TValue>([NotNull] ObjectContext context, [NotNull] Func<TValue> factory, [NotNull] string name)
            where TKey : class, IBaseHolder where TValue : class
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(factory != null, "factory");
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Ensures(Contract.Result<TValue>() != null);

            var instance = context.GetAll<TKey>().FirstOrDefault(holder => holder.Name == name) as TValue;

            if (instance != null) return instance;

            instance = factory();
            context.Register<TKey, TValue>(instance);
            return instance;
        }

        #endregion
    }

    /// <summary>
    ///     The base holder.
    /// </summary>
    /// <typeparam name="TValue">
    /// </typeparam>
    public abstract class BaseHolder<TValue> : IBaseHolder, IDisposable
        where TValue : class
    {
        #region Fields

        private TValue _value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseHolder{TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BaseHolder{TValue}" /> Klasse.
        ///     Initializes a new instance of the <see cref="BaseHolder{TValue}" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected BaseHolder([NotNull] TValue value)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");

            Value = value;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="BaseHolder{TValue}" /> class.
        ///     Finalisiert eine Instanz der <see cref="BaseHolder{TValue}" /> Klasse.
        /// </summary>
        ~BaseHolder()
        {
            Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        [NotNull]
        public TValue Value
        {
            get
            {
                Contract.Ensures(Contract.Result<TValue>() != null);

                return _value;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _value = value;
            }
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            var dipo = Value as IDisposable;
            if (dipo != null) dipo.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}