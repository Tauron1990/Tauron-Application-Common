// The file ScheduleAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ScheduleAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The task option.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The task option.</summary>
    public enum TaskOption
    {
        /// <summary>The worker.</summary>
        Worker,

        /// <summary>The task.</summary>
        Task,

        /// <summary>The ui thread.</summary>
        UIThread
    }

    /// <summary>The schedule attribute.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ScheduleAttribute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _is ok.</summary>
        private bool _isOk;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ScheduleAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ScheduleAttribute" /> class.
        /// </summary>
        public ScheduleAttribute()
        {
            Order = 0;
            CreationOptions = TaskCreationOptions.None;
            TaskOption = TaskOption.Worker;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the creation options.</summary>
        /// <value>The creation options.</value>
        public TaskCreationOptions CreationOptions { get; set; }

        /// <summary>Gets or sets the task option.</summary>
        /// <value>The task option.</value>
        public TaskOption TaskOption { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IInterceptor" />.
        /// </returns>
        [NotNull]
        public override IInterceptor Create([NotNull] MemberInfo info)
        {
            _isOk = ((MethodInfo) info).ReturnType == typeof (void);

            if (Logger.IsLoggingEnabled() && !_isOk)
            {
                CommonConstants.LogCommon(false,
                                          "AOP Module: The member {0}.{1} has no Void Return",
                                          info.DeclaringType.FullName,
                                          info.Name);
            }

            return base.Create(info);
        }

        #endregion

        #region Methods

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
            if (!_isOk)
            {
                invocation.Proceed();
                return;
            }

            switch (TaskOption)
            {
                case TaskOption.Worker:
                    CommonApplication.QueueWorkitem(invocation.Proceed, false);
                    break;
                case TaskOption.Task:
                    Task.Factory.StartNew(invocation.Proceed, CreationOptions);
                    break;
                case TaskOption.UIThread:
                    CommonApplication.QueueWorkitem(invocation.Proceed, true);
                    break;
                default:
                    CommonConstants.LogCommon(false, "Invalid Schedule TaskOption: {0}.{1}", invocation.TargetType, invocation.Method);
                    invocation.Proceed();
                    break;
            }
        }

        #endregion
    }
}