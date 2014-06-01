#region

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop
{
    /// <summary>The property aspect attrebute.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyAspectAttrebute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _property info.</summary>
        private PropertyInfo _propertyInfo;

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
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            string name = invocation.Method.Name;
            bool getter = name.StartsWith(AopConstants.PropertyGetter, StringComparison.Ordinal);
            if (_propertyInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.PropertyGetter.Length : AopConstants.PropertySetter.Length);

                _propertyInfo = invocation.Method.DeclaringType.GetProperty(name, AopConstants.DefaultBindingFlags);
                Contract.Assert(_propertyInfo != null);
            }

            if (getter) OnGet(invocation, context, _propertyInfo);
            else OnSet(invocation, context, _propertyInfo);
        }

        /// <summary>
        ///     The on get.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property.
        /// </param>
        protected virtual void OnGet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] PropertyInfo propertyInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(propertyInfo != null, "propertyInfo");

            invocation.Proceed();
        }

        /// <summary>
        ///     The on set.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property.
        /// </param>
        protected virtual void OnSet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] PropertyInfo propertyInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(propertyInfo != null, "propertyInfo");

            invocation.Proceed();
        }

        #endregion
    }
}