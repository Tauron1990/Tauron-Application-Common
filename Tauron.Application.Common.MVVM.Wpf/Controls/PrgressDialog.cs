using System;
using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;

namespace Tauron.Application.Controls
{
    public sealed class SimpleProgressDialog : IProgressDialog
    {
        public SimpleProgressDialog([NotNull] string text, [NotNull] string title, [NotNull] IWindow owner, [NotNull] Action<IProgress<ActiveProgress>> worker)
        {
            _owner = owner;
            _worker = worker;
            _dialog = new ProgressDialog
            {
                Text = text,
                ShowTimeRemaining = false,
                WindowTitle = title,
                ShowCancelButton = false
            };
            _dialog.DoWork += DoWork;
            _dialog.RunWorkerCompleted += RunWorkerCompleted;
        }

        public event EventHandler Completed;

        public ProgressStyle ProgressBarStyle
        {
            get => (ProgressStyle) _dialog.ProgressBarStyle;
            set => _dialog.ProgressBarStyle = (ProgressBarStyle) value;
        }

        private class DialogReporter : IProgress<ActiveProgress>
        {
            public DialogReporter([NotNull] ProgressDialog dialog, [NotNull] string text)
            {
                _dialog = Argument.NotNull(dialog, nameof(dialog));
                _text = Argument.NotNull(text, nameof(text));
            }

            public void Report([NotNull] ActiveProgress value) => _dialog.ReportProgress((int) value.OverAllProgress, _text, $"{value.Message} : {(int) value.Percent}%");

            private readonly ProgressDialog _dialog;
            private readonly string _text;

        }
        
        private readonly ProgressDialog _dialog;
        private readonly IWindow _owner;
        private readonly Action<IProgress<ActiveProgress>> _worker;

        public void Dispose() => _dialog.Dispose();

        public void Start()
        {
            ObservableObject.CurrentDispatcher.Invoke(() =>
                {
                    if (_owner == null) _dialog.ShowDialog();
                    else
                    {
                        _dialog.ShowDialog((Window)_owner.TranslateForTechnology());
                    }
                });
        }

        private void DoWork([NotNull] object sender, [NotNull] DoWorkEventArgs e) => _worker(new DialogReporter(_dialog, _dialog.Description));

        private void RunWorkerCompleted([NotNull] object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            Dispose();
            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}