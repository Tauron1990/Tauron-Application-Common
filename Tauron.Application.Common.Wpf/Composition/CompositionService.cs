// The file CompositionService.cs is part of Tauron.Application.Common.Wpf.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;
using IContainer = Tauron.Application.Ioc.IContainer;

// <copyright file="CompositionService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The composition services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The composition services.</summary>
    [PublicAPI]
    public static class CompositionServices
    {
        #region Static Fields

        public static readonly DependencyProperty ImportViewModelProperty =
            DependencyProperty.RegisterAttached("ImportViewModel", typeof (string), typeof (CompositionServices),
                                                new PropertyMetadata(default(string), ImportViewModelPropertyChanged));

        public static void SetImportViewModel([NotNull] DependencyObject element, [CanBeNull] string value)
        {
            element.SetValue(ImportViewModelProperty, value);
        }

        [NotNull]
        public static string GetImportViewModel([NotNull] DependencyObject element)
        {
            return (string) element.GetValue(ImportViewModelProperty);
        }

        public static readonly DependencyProperty ImportProperty = DependencyProperty.RegisterAttached(
            "Import",
            typeof (Type),
            typeof (
                CompositionServices
                ),
            new PropertyMetadata
                (null,
                 ImportPropertyChanged));

        private static IContainer _container;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the container.</summary>
        [NotNull]
        public static IContainer Container
        {
            get { return _container ?? (_container = new DefaultContainer()); }

            set { _container = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get import.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        [CanBeNull]
        public static Type GetImport([NotNull] DependencyObject obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            return (Type) obj.GetValue(ImportProperty);
        }

        /// <summary>
        ///     The set import.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetImport([NotNull] DependencyObject obj, [CanBeNull] Type value)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            obj.SetValue(ImportProperty, value);
        }

        #endregion

        #region Methods

        private static void ImportPropertyChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            try
            {
                IEnumerable<IViewAggregatorAdapter> adapters = Container.ResolveAll<IViewAggregatorAdapter>(null);
                IViewAggregatorAdapter adapter = adapters.FirstOrDefault(a => a.CanAdapt(d));

                if (adapter == null)
                {
                    var obj = new FrameworkObject(d);
                    IEnumerable<object> views = Container.ResolveAll((Type) e.NewValue, null);
                    obj.DataContext = views.FirstOrDefault();
                    return;
                }

                lock (adapter)
                {
                    adapter.Adapt(d);
                    IEnumerable<object> views = Container.ResolveAll((Type) e.NewValue, null);

                    adapter.AddViews(views);

                    adapter.Release();
                }
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.HandleException(ex, CommonWpfConstans.CommonExceptionPolicy)) throw;
            }
        }

        private static void ImportViewModelPropertyChanged([NotNull] DependencyObject d,
                                                           DependencyPropertyChangedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(d))
                return;

            var name = e.NewValue as string;
            
            if(string.IsNullOrWhiteSpace(name)) return;

            new FrameworkObject(d, false)
            {
                DataContext = Container.Resolve(typeof (ViewModelBase), name, true, null)
            };
        }

        #endregion
    }
}