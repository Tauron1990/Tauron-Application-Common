using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Tauron.Application.Commands;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The tab workspace.</summary>
    public abstract class TabWorkspace : ViewModelBase, ITabWorkspace
    {
        #region Constants

        /// <summary>The close event name.</summary>
        protected const string CloseEventName = "CloseEvent";

        #endregion

        #region Fields

        private bool _canClose;

        private string _tile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabWorkspace" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TabWorkspace" /> Klasse.
        /// </summary>
        /// <param name="title">
        ///     The title.
        /// </param>
        protected TabWorkspace([NotNull] string title)
        {
            _tile = title;
            _canClose = true;
            CloseWorkspace = new SimpleCommand(obj => CanClose, obj => InvokeClose());
        }

        #endregion

        #region Public Events

        /// <summary>The close.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ITabWorkspace> Close
        {
            add { AddEvent(CloseEventName, value); }

            remove { RemoveEvent(CloseEventName, value); }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether can close.</summary>
        public bool CanClose
        {
            get { return _canClose; }

            set
            {
                _canClose = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the close workspace.</summary>
        public ICommand CloseWorkspace { get; private set; }

        /// <summary>Gets or sets the title.</summary>
        public string Title
        {
            get { return _tile; }

            set
            {
                _tile = value;
                OnPropertyChanged();
            }
        }

        public abstract void OnClose();
        public abstract void OnActivate();
        public abstract void OnDeactivate();

        #endregion

        #region Public Methods and Operators

        /// <summary>The invoke close.</summary>
        public virtual void InvokeClose()
        {
            InvokeEvent(CloseEventName, this);
        }

        #endregion
    }
}