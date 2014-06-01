#region

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The context property attribute base.</summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ContextPropertyAttributeBase : ObjectContextPropertyAttribute
    {
        #region Fields

        private string _holderName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ContextPropertyAttributeBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        /// </summary>
        protected ContextPropertyAttributeBase()
        {
            HolderName = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the holder name.</summary>
        /// <value>The holder name.</value>
        [NotNull]
        public string HolderName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _holderName;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _holderName = value;
            }
        }

        #endregion
    }
}