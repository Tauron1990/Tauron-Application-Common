#region

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The user task.</summary>
    public class UserTask : ITask
    {
        #region Fields

        /// <summary>The _callback.</summary>
        private readonly Action _callback;

        private readonly TaskCompletionSource<object> _task;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="UserTask" /> Klasse.
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        /// </summary>
        /// <param name="callback">
        ///     The callback.
        /// </param>
        /// <param name="sync">
        ///     The sync.
        /// </param>
        public UserTask([NotNull] Action callback, bool sync)
        {
            Contract.Requires<ArgumentNullException>(callback != null, "callback");

            _callback = callback;
            Synchronize = sync;
            _task = new TaskCompletionSource<object>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        public bool Synchronize { get; private set; }

        /// <summary>Gets the task.</summary>
        [NotNull]
        public Task Task
        {
            get { return _task.Task; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        public void Execute()
        {
            try
            {
                _callback();
                _task.SetResult(null);
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        #endregion
    }
}