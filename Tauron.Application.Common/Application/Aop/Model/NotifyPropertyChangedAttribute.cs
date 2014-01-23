// The file NotifyPropertyChangedAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="NotifyPropertyChangedAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The notify property changed attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The notify property changed attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NotifyPropertyChangedAttribute : AspectBaseAttribute
    {
        #region Fields

        private Action<string> _eventInvoker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyPropertyChangedAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="NotifyPropertyChangedAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="NotifyPropertyChangedAttribute" /> class.
        /// </summary>
        public NotifyPropertyChangedAttribute()
        {
            AlternativePropertyChangedName = "OnPropertyChanged";
            Order = 900;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the alternative property changed name.</summary>
        /// <value>The alternative property changed name.</value>
        [NotNull]
        public string AlternativePropertyChangedName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        [ContractVerification(false)]
        protected internal override void Initialize([NotNull] object target, [NotNull] ObjectContext context,
                                                    [NotNull] string contextName)
        {
            var metod = target as INotifyPropertyChangedMethod;
            if (metod != null) _eventInvoker = metod.OnPropertyChanged;
            else
            {
                MethodInfo info =
                    target.GetType()
                          .GetMethods(AopConstants.DefaultBindingFlags)
                          .FirstOrDefault(
                              metodInfo =>
                              metodInfo.Name == AlternativePropertyChangedName
                              && metodInfo.ReturnType == typeof (void));
                if (info != null && info.GetParameters().Length == 1)
                {
                    Type parameterType = info.GetParameters()[0].ParameterType;
                    if (parameterType == typeof (PropertyChangedEventArgs)) _eventInvoker = s => info.Invoke(target, new PropertyChangedEventArgs(s));
                    else if (parameterType == typeof (string)) _eventInvoker = s => info.Invoke(target, s);
                    else
                        CommonConstants.LogCommon(false, "AOP Module: No PropertyChanged Method Found: Class:{0} AltName:{1}",
                                                  target,
                                                  AlternativePropertyChangedName);
                }
            }

            base.Initialize(target, context, contextName);
        }

        /// <summary>
        ///     The intercept impl.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected override void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            invocation.Proceed();

            if (_eventInvoker == null) return;

            if (invocation.Method.Name.StartsWith(AopConstants.PropertySetter, StringComparison.Ordinal)
                && invocation.Method.IsSpecialName) _eventInvoker(invocation.Method.Name.Remove(0, AopConstants.PropertySetter.Length));
        }

        #endregion
    }
}