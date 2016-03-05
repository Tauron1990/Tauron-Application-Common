using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Threading;
using Tauron.Application.Ioc;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public abstract class ViewModelBase : ModelBase
    {
        [PublicAPI]
        protected class LinkedProperty : IDisposable
        {
            private ObservableObject _host;
            private string _name;
            private INotifyPropertyChanged _target;
            private readonly string _custom;

            public LinkedProperty(ObservableObject host, string name, INotifyPropertyChanged target, string custom)
            {
                _host = host;
                _name = name;
                _target = target;
                _custom = custom;

                _target.PropertyChanged += PropertyChangedMethod;
            }

            private void PropertyChangedMethod(object sender, PropertyChangedEventArgs e)
            {
                if(e.PropertyName != _name) return;

                _host.OnPropertyChangedExplicit(_custom ?? _name);
            }
            
            public void Stop()
            {
                if(_target == null) return;

                _target.PropertyChanged -= PropertyChangedMethod;

                _host = null;
                _name = null;
                _target = null;
            }

            public void Dispose()
            {
                Stop();
            }
        }

        [NotNull]
        public static ViewModelBase ResolveViewModel([NotNull] string name)
        {
            return CommonApplication.Current.Container.Resolve<ViewModelBase>(name, false);
        }

        protected ViewModelBase()
        {
            ModelList = new Dictionary<string, ModelBase>();
        }

        [NotNull]
        protected Dictionary<string, ModelBase> ModelList { get; private set; }

        internal void RegisterInheritedModel([NotNull] string name, [NotNull] ModelBase model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            model.PropertyChanged += ModelOnPropertyChanged;
            ModelList.Add(name, model);
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(propertyChangedEventArgs);
        }

        [NotNull]
        internal ModelBase GetModelBase([NotNull] string name)
        {
            return ModelList[name];
        }

        internal bool ContainsModel([NotNull] string name)
        {
            return ModelList.ContainsKey(name);
        }


        protected bool UnRegisterInheritedModel([NotNull] string model)
        {
            GetModelBase(model).PropertyChanged -= ModelOnPropertyChanged;
            return ModelList.Remove(model);
        }

        protected LinkedProperty LinkProperty(string name, INotifyPropertyChanged target, string customName = null)
        {
            return new LinkedProperty(this, name, target, customName);
        }

        protected LinkedProperty LinkPropertyExp<T>(Expression<Func<T>> name, INotifyPropertyChanged target, string customName = null)
        {
            return new LinkedProperty(this, PropertyHelper.ExtractPropertyName(name), target, customName);
        }

        [NotNull, Inject]
        public ViewManager ViewManager { get; protected set; }

        [NotNull, Inject]
        public IDialogFactory Dialogs { get; protected set; }

        [NotNull]
        public System.Windows.Application CurrentApplication => System.Windows.Application.Current;

        [NotNull]
        public Dispatcher SystemDispatcher => CurrentApplication.Dispatcher;

        [CanBeNull]
        public IWindow MainWindow => CommonApplication.Current.MainWindow;

        [NotNull]
        public IUISynchronize Synchronize => UiSynchronize.Synchronize;

        protected override IEnumerable<ObservableProperty> CustomObservableProperties()
        {
            return ModelList.Values.SelectMany(m => GetProperties(m.GetType()));
        }

        protected bool EditingInheritedModel { get; set; }

        public override void BeginEdit()
        {
            if(EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.BeginEdit();
            base.BeginEdit();
        }

        public override void EndEdit()
        {
            if(EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.EndEdit();
            base.EndEdit();
        }

        public override void CancelEdit()
        {
            if(EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.CancelEdit();
            base.CancelEdit();
        }

        protected override bool HasErrorOverride => ModelList.Values.Any(m => m.HasErrors);
        protected override IEnumerable GetErrorsOverride(string property)
        {
            var first = ModelList.Values.FirstOrDefault(mb => mb.GetIssuesDictionary().ContainsKey(property));

            return first?.GetIssuesDictionary()[property];
        }
    }
}