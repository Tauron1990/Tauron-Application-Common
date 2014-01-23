using System;
using System.Collections.Generic;
using Tauron.Application.Ioc;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [Intercept]
    public abstract class ViewModelBase : ModelBase
    {
        protected ViewModelBase()
        {
            ModelList = new Dictionary<string, ModelBase>();
        }

        [NotNull]
        protected Dictionary<string, ModelBase> ModelList { get; private set; }

        internal void RegisterInheritedModel([NotNull] string name, [NotNull] ModelBase model)
        {
            if (model == null) throw new ArgumentNullException("model");

            ModelList.Add(name, model);
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
            return ModelList.Remove(model);
        }

        [NotNull, Inject]
        public ViewManager ViewManager { get; protected set; }

        [NotNull, Inject]
        public IDialogFactory Dialogs { get; protected set; }
    }
}