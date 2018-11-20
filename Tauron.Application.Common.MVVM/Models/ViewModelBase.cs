using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Tauron.Application.Commands;
using Tauron.Application.Ioc;
using Tauron.Application.Models.Interfaces;
using Tauron.Application.UIConnector;
using Tauron.Application.Views;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public abstract class ViewModelBase : ModelBase, IShowInformation
    {
        [CanBeNull]
        public static IApplicationConnector ApplicationConnector 
            => _applicationConnector ?? (_applicationConnector = CommonApplication.Current.Container.Resolve<IUIConnector>().ApplicationConnector);


        private static IDialogFactory _dialogs;
        private static IApplicationConnector _applicationConnector;

        // ReSharper disable once NotNullMemberIsNotInitialized
        protected ViewModelBase() => ModelList = new Dictionary<string, ModelBase>();

        public static bool IsInDesignMode => ApplicationConnector?.IsInDesignMode ?? false;

        [NotNull]
        protected Dictionary<string, ModelBase> ModelList { get; private set; }

        [NotNull]
        [Inject]
        public ViewManager ViewManager { get; protected set; }

        [NotNull]
        public static IDialogFactory Dialogs => _dialogs ?? (_dialogs = CommonApplication.Current.Container.Resolve<IDialogFactory>());

        [NotNull]
        public IApplication CurrentApplication => Argument.CheckResult(ApplicationConnector, "No Application interface").Application;

        [NotNull]
        public IDispatcher SystemDispatcher => Argument.CheckResult(ApplicationConnector, "No Application interface").Dispatcher;

        [CanBeNull]
        public static IWindow MainWindow => CommonApplication.Current.MainWindow;

        [NotNull]
        public IUISynchronize Synchronize => UiSynchronize.Synchronize;

        protected bool EditingInheritedModel { get; set; }

        protected override bool HasErrorOverride => ModelList.Values.Any(m => m.HasErrors);

        public virtual void OnShow(IWindow window) { }

        public virtual void AfterShow(IWindow window) { }

        [NotNull]
        public static ViewModelBase ResolveViewModel([NotNull] string name) => CommonApplication.Current.Container.Resolve<ViewModelBase>(name, false);

        protected internal void RegisterInheritedModel([NotNull] string name, [NotNull] ModelBase model)
        {
            Argument.NotNull(model, nameof(model));

            model.PropertyChanged += ModelOnPropertyChanged;

            ModelList.Add(name, model);
        }

        protected virtual void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) => OnPropertyChanged(this, propertyChangedEventArgs);

        [NotNull]
        internal ModelBase GetModelBase([NotNull] string name) => ModelList[name];

        internal bool ContainsModel([NotNull] string name) => ModelList.ContainsKey(name);


        protected bool UnRegisterInheritedModel([NotNull] string model)
        {
            GetModelBase(model).PropertyChanged -= ModelOnPropertyChanged;
            return ModelList.Remove(model);
        }

        protected LinkedProperty LinkProperty(string name, INotifyPropertyChanged target, string customName = null) => new LinkedProperty(this, name, target, customName);

        protected LinkedProperty LinkPropertyExp<T>(Expression<Func<T>> name, INotifyPropertyChanged target, string customName = null) => new LinkedProperty(this, PropertyHelper.ExtractPropertyName(name), target, customName);

        protected override IEnumerable<ObservablePropertyDescriptor> CustomObservableProperties() => ModelList.Values.SelectMany(m => m.GetPropertyDescriptors());

        public override void BeginEdit()
        {
            if (EditingInheritedModel)
            {
                foreach (var value in ModelList.Values)
                    value.BeginEdit();
            }

            base.BeginEdit();
        }

        public override void EndEdit()
        {
            if (EditingInheritedModel)
            {
                foreach (var value in ModelList.Values)
                    value.EndEdit();
            }

            base.EndEdit();
        }

        public override void CancelEdit()
        {
            if (EditingInheritedModel)
            {
                foreach (var value in ModelList.Values)
                    value.CancelEdit();
            }

            base.CancelEdit();
        }

        protected override IEnumerable GetErrorsOverride(string property)
        {
            var first = ModelList.Values.FirstOrDefault(mb => mb.GetIssuesDictionary().ContainsKey(property));

            return first?.GetIssuesDictionary()[property];
        }

        protected void InvalidateRequerySuggested() => 
            CommonApplication.Scheduler.QueueTask(new UserTask(() => CurrentDispatcher.BeginInvoke(CommandManagerDelegator.InvalidateRequerySuggested), false));

        protected override void OnErrorsChanged(string propertyName) => 
            CommonApplication.Scheduler.QueueTask(new UserTask(() => { Synchronize.Invoke(() => base.OnErrorsChanged(propertyName)); }, false));

        protected Bundle<TType> CreateBundle<TType>([NotNull] string name, [CanBeNull] ObservablePropertyMetadata metadata, params ModelRule[] rules)
        {
            if(metadata == null && rules?.Length != 0)
                metadata = new ObservablePropertyMetadata();
            if(rules != null)
                metadata?.SetValidationRules(rules);

            return new Bundle<TType>(this, RegisterProperty(name, GetType(), typeof(TType), metadata));
        }
        protected Bundle<TType> CreateBundle<TType>([NotNull] string name, params ModelRule[] rules)
            => CreateBundle<TType>(name, null, rules);

        [PublicAPI]
        protected class LinkedProperty : IDisposable
        {
            private readonly string _custom;
            private ObservableObject _host;
            private string _name;
            private INotifyPropertyChanged _target;

            public LinkedProperty(ObservableObject host, string name, INotifyPropertyChanged target, string custom)
            {
                _host = host;
                _name = name;
                _target = target;
                _custom = custom;

                _target.PropertyChanged += PropertyChangedMethod;
            }

            public void Dispose() => Stop();

            private void PropertyChangedMethod(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != _name) return;

                _host.OnPropertyChangedExplicit(_custom ?? _name);
            }

            public void Stop()
            {
                if (_target == null) return;

                _target.PropertyChanged -= PropertyChangedMethod;

                _host = null;
                _name = null;
                _target = null;
            }
        }
    }
}