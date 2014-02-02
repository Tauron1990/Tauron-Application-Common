// The file MethodCommand.cs is part of Tauron.Application.Common.Wpf.
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
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodCommand.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Kapselt die Event Daten fals es sich bei dem ziel um ein Commando Handelt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>
    ///     Kapselt die Event Daten fals es sich bei dem ziel um ein Commando Handelt.
    /// </summary>
    public sealed class EventData
    {
        #region Constructors and Destructors

        internal EventData(object sender, EventArgs eventArgs)
        {
            Contract.Requires<ArgumentNullException>(sender != null, "sender");
            Contract.Requires<ArgumentNullException>(eventArgs != null, "eventArgs");

            Sender = sender;
            EventArgs = eventArgs;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Die Ereignis Daten
        /// </summary>
        [NotNull]
        public EventArgs EventArgs { get; private set; }

        /// <summary>
        ///     Das Objekt, an das der Ereignishandler angefügt wird.
        /// </summary>
        [NotNull]
        public object Sender { get; private set; }

        #endregion
    }

    /// <summary>The method command.</summary>
    public sealed class MethodCommand : CommandBase
    {
        #region Fields

        private readonly WeakReference context;

        private readonly MethodInfo method;

        private readonly MethodType methodType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCommand" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MethodCommand" /> Klasse.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        public MethodCommand(MethodInfo method, WeakReference context)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            this.method = method;
            this.context = context;

            methodType = (MethodType) method.GetParameters().Count();
            if (methodType == MethodType.One) if (this.method.GetParameters()[0].ParameterType != typeof (EventData)) methodType = MethodType.EventArgs;
        }

        #endregion

        #region Enums

        private enum MethodType
        {
            /// <summary>The zero.</summary>
            Zero = 0,

            /// <summary>The one.</summary>
            One,

            /// <summary>The two.</summary>
            Two,

            /// <summary>The event args.</summary>
            EventArgs,
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the context.</summary>
        public object Context
        {
            get { return context == null ? null : context.Target; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public override void Execute(object parameter)
        {
            var temp = (EventData) parameter;
            switch (methodType)
            {
                case MethodType.Zero:
                    method.Invoke(Context, new object[0]);
                    break;
                case MethodType.One:
                    method.Invoke(Context, new object[] {temp});
                    break;
                case MethodType.Two:
                    method.Invoke(Context, new[] {temp.Sender, temp.EventArgs});
                    break;
                case MethodType.EventArgs:
                    method.Invoke(Context, new object[] {temp.EventArgs});
                    break;
            }
        }

        #endregion
    }
}