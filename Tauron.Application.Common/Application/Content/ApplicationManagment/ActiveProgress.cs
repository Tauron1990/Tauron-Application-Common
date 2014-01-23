// The file ActiveProgress.cs is part of Tauron.Application.Common.
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
// <copyright file="ActiveProgress.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The active progress.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The active progress.</summary>
    [PublicAPI]
    public class ActiveProgress
    {
        #region Fields

        private string message;

        private double overAllProgress;

        private double percent;

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
        public ActiveProgress(string message, double percent, double overAllProgress)
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
        public string Message
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return message;
            }

            private set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                message = value;
            }
        }

        /// <summary>Gets or sets the over all progress.</summary>
        /// <value>The over all progress.</value>
        public double OverAllProgress
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0 && Contract.Result<double>() <= 100);

                return overAllProgress;
            }

            set
            {
                Contract.Requires<ArgumentException>(value >= 0 && value <= 100, "OverAllProgress");

                overAllProgress = value;
            }
        }

        /// <summary>Gets the percent.</summary>
        /// <value>The percent.</value>
        public double Percent
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0 && Contract.Result<double>() <= 100);

                return percent;
            }

            private set
            {
                Contract.Requires<ArgumentException>(value >= 0 && value <= 100, "Percent");

                percent = value;
            }
        }

        #endregion
    }
}