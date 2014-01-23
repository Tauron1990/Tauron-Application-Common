// The file SplashServices.cs is part of Tauron.Application.Common.
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
// <copyright file="SplashServices.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The splash message listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The splash message listener.</summary>
    public sealed class SplashMessageListener : ObservableObject
    {
        #region Fields

        /// <summary>The _message.</summary>
        private string _message;

        /// <summary>The _splash content.</summary>
        private object _splashContent;

        private double _height = 236;

        private string _mainLabelForeground = "Black";

        private string _mainLabelbackground = "White";

        private double _widht = 414;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplashMessageListener" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SplashMessageListener" /> Klasse.
        /// </summary>
        public SplashMessageListener()
        {
            CurrentListner = this;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the current listner.</summary>
        [NotNull]
        public static SplashMessageListener CurrentListner { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        public double Height
        {
            get { return _height; }

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the main label foreground.
        /// </summary>
        [NotNull]
        public string MainLabelForeground
        {
            get { return _mainLabelForeground; }

            set
            {
                _mainLabelForeground = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the main labelbackground.
        /// </summary>
        [NotNull]
        public string MainLabelbackground
        {
            get { return _mainLabelbackground; }

            set
            {
                _mainLabelbackground = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Get or set received message.</summary>
        /// <value>The message.</value>
        [NotNull]
        public string Message
        {
            get { return _message; }

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the splash content.</summary>
        /// <value>The splash content.</value>
        [NotNull]
        public object SplashContent
        {
            get { return _splashContent; }

            set
            {
                _splashContent = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the widht.
        /// </summary>
        public double Widht
        {
            get { return _widht; }

            set
            {
                _widht = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The receive message.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void ReceiveMessage([NotNull] string message)
        {
            Message = message;
        }

        #endregion
    }

    /// <summary>The SplashService interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (SplashServiceContracts))]
    public interface ISplashService
    {
        #region Public Properties

        /// <summary>Gets the listner.</summary>
        /// <value>The listner.</value>
        [NotNull]
        SplashMessageListener Listner { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close splash.</summary>
        void CloseSplash();

        /// <summary>The show splash.</summary>
        void ShowSplash();

        #endregion
    }

    [ContractClassFor(typeof (ISplashService))]
    internal abstract class SplashServiceContracts : ISplashService
    {
        #region Public Properties

        /// <summary>Gets the listner.</summary>
        /// <value>The listner.</value>
        /// <exception cref="NotImplementedException"></exception>
        public SplashMessageListener Listner
        {
            get
            {
                Contract.Ensures(Contract.Result<SplashMessageListener>() != null);
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close splash.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CloseSplash()
        {
            throw new NotImplementedException();
        }

        /// <summary>The show splash.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ShowSplash()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}