// The file ContextManager.cs is part of Tauron.Application.Common.
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
// <copyright file="ContextManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The context manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The context manager.</summary>
    [PublicAPI]
    public static class ContextManager
    {
        #region Static Fields

        /// <summary>The _aspect contexts.</summary>
        private static readonly Dictionary<string, WeakContext> _aspectContexts = Initialize();

        /// <summary>The _weak contexts.</summary>
        private static WeakReferenceCollection<WeakContext> _weakContexts;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find context.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext FindContext(object target, string contextName)
        {
            Contract.Ensures(Contract.Result<ObjectContext>() != null);

            if (contextName != null)
            {
                WeakContext weakHolder = _aspectContexts[contextName];
                Contract.Assume(weakHolder != null);
                ObjectContext context = weakHolder.Context;
                Contract.Assume(context != null);
                return context;
            }

            var holder = target as IContextHolder;
            if (holder != null) return holder.Context;

            WeakContext temp = _weakContexts.FirstOrDefault(con => ReferenceEquals(target, con.Owner));
            if (temp == null) throw new InvalidOperationException();

            return temp.Context;
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext GetContext(string name, object owner)
        {
            Contract.Requires<ArgumentNullException>(owner != null, "owner");
            Contract.Ensures(Contract.Result<ObjectContext>() != null);

            WeakContext context;
            if (_aspectContexts.TryGetValue(name, out context))
            {
                Contract.Assume(context != null);

                return context.Context;
            }

            var tempContext = new ObjectContext();
            AddContext(name, tempContext, owner);

            return tempContext;
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext GetContext(object target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");
            Contract.Ensures(Contract.Result<ObjectContext>() != null);

            var context = new ObjectContext();
            _weakContexts.Add(new WeakContext(target));
            return context;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The add context.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        private static void AddContext(string name, ObjectContext context, object owner)
        {
            Contract.Requires<ArgumentNullException>(owner != null, "owner");

            _aspectContexts[name] = new WeakContext(owner) {Context = context};
        }

        /// <summary>The clean contexts.</summary>
        private static void CleanContexts()
        {
            lock (_aspectContexts)
            {
                string[] reference =
                    _aspectContexts.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToArray();
                foreach (string equalableWeakReference in reference) _aspectContexts.Remove(equalableWeakReference);
            }
        }

        /// <summary>The initialize.</summary>
        /// <returns>
        ///     The <see cref="Dictionary" />.
        /// </returns>
        private static Dictionary<string, WeakContext> Initialize()
        {
            WeakCleanUp.RegisterAction(CleanContexts);
            _weakContexts = new WeakReferenceCollection<WeakContext>();
            return new Dictionary<string, WeakContext>();
        }

        #endregion

        /// <summary>The weak context.</summary>
        private class WeakContext : IWeakReference
        {
            #region Fields

            /// <summary>The _holder.</summary>
            private readonly WeakReference _holder;

            private ObjectContext context;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="WeakContext" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="WeakContext" /> Klasse.
            ///     Initializes a new instance of the <see cref="WeakContext" /> class.
            /// </summary>
            /// <param name="owner">
            ///     The owner.
            /// </param>
            public WeakContext(object owner)
            {
                Contract.Requires<ArgumentNullException>(owner != null, "owner");

                _holder = new WeakReference(owner);
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the context.</summary>
            /// <value>The context.</value>
            public ObjectContext Context
            {
                get
                {
                    Contract.Ensures(Contract.Result<ObjectContext>() != null);

                    return context;
                }

                set
                {
                    Contract.Requires<ArgumentNullException>(value != null, "value");

                    context = value;
                }
            }

            /// <summary>Gets the owner.</summary>
            /// <value>The owner.</value>
            public object Owner
            {
                get { return _holder.Target; }
            }

            /// <summary>Gets a value indicating whether is alive.</summary>
            /// <value>The is alive.</value>
            public bool IsAlive
            {
                get { return _holder.IsAlive; }
            }

            #endregion
        }
    }
}