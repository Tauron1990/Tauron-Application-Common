using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The active progress.</summary>
    [PublicAPI]
    public class ActiveProgress
    {
        #region Fields

        private string _message;

        private double _overAllProgress;

        private double _percent;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActiveProgress" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ActiveProgress" /> Klasse.
        ///     Initializes a new instance of the <see cref="ActiveProgress" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="percent">
        ///     The percent.
        /// </param>
        /// <param name="overAllProgress">
        ///     The over all progress.
        /// </param>
        public ActiveProgress([NotNull] string message, double percent, double overAllProgress)
        {
            if (percent < 0) percent = 0;
            if (overAllProgress < 0) overAllProgress = 0;

            if (percent > 100 || double.IsNaN(percent)) percent = 100;

            if (overAllProgress > 100 || double.IsNaN(overAllProgress)) overAllProgress = 100;

            Message = message;
            Percent = percent;
            OverAllProgress = overAllProgress;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the message.</summary>
        /// <value>The message.</value>
        [NotNull]
        public string Message
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _message;
            }

            private set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _message = value;
            }
        }

        /// <summary>Gets or sets the over all progress.</summary>
        /// <value>The over all progress.</value>
        public double OverAllProgress
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0 && Contract.Result<double>() <= 100);

                return _overAllProgress;
            }

            set
            {
                Contract.Requires<ArgumentException>(value >= 0 && value <= 100, "OverAllProgress");

                _overAllProgress = value;
            }
        }

        /// <summary>Gets the percent.</summary>
        /// <value>The percent.</value>
        public double Percent
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0 && Contract.Result<double>() <= 100);

                return _percent;
            }

            private set
            {
                Contract.Requires<ArgumentException>(value >= 0 && value <= 100, "Percent");

                _percent = value;
            }
        }

        #endregion
    }
}