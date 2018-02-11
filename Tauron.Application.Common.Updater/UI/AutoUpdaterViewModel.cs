using System;
using System.Windows;
using System.Windows.Documents;
using Tauron.Application.Common.Updater;
using Tauron.Application.Common.Updater.Impl;
using Tauron.Application.Common.Updater.Provider;
using Tauron.Application.Common.Updater.Service;
using Tauron.Application.Models;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows
{
    [ExportViewModel(UpdaterConststands.UpdateView)]
    public sealed class AutoUpdaterViewModel : ViewModelBase
    {
        private bool _updateFound;
        private bool _upadeRunning;
        private IUpdate _update;

        private FlowDocument _updateDescription;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private double _updateProgress;
        private string _updateText;

        public FlowDocument UpdateDescription
        {
            get => _updateDescription;
            set { _updateDescription = value; OnPropertyChanged(); }
        }
        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set { _progressVisibility = value; OnPropertyChanged();}
        }
        public double UpdateProgress
        {
            get => _updateProgress;
            set { _updateProgress = value; OnPropertyChanged();}
        }
        public string UpdateText
        {
            get => _updateText;
            set { _updateText = value; OnPropertyChanged();}
        }

        public IUpdateManager UpdateManager { get; } = UpdaterService.UpdateManager;

        [CommandTarget]
        public bool CanCheckUpdate()
        {
            return !_upadeRunning;
        }

        [CommandTarget]
        public void CheckUpdate()
        {
            if(_updateFound) return;

            var update = UpdateManager.CheckUpdate();
            if(update == null) return;

            _updateFound = true;
            _update = update;

            Synchronize.BeginInvoke(BuildDescription);
            InvalidateRequerySuggested();
        }

        [CommandTarget]
        public void InstallUpdate()
        {
            _upadeRunning = true;
            ProgressVisibility = Visibility.Visible;
            OnLockUIEvent();
            InvalidateRequerySuggested();

            UpdateManager.InstallUpdate(_update);
            OnShutdownEvent();
        }

        [CommandTarget]
        public bool CanInstallUpdate()
        {
            return !_upadeRunning && _updateFound;
        }

        private void BuildDescription()
        {
            UpdateDescription = new FlowDocument(new Paragraph(new Run(_update.Release.Description)));
        }

        public override void BuildCompled()
        {
            UpdaterService.Configuration.Provider.Preperator.PreperationInProgressEvent += PreperatorOnPreperationInProgressEvent;
            UpdaterService.Configuration.Provider.Downloader.ProgressEvent += DownloaderOnProgressEvent;
        }

        private void DownloaderOnProgressEvent(object sender, DownloadProgressEventArgs downloadProgressEventArgs)
        {
            ProgressChanged(downloadProgressEventArgs.Percent, Messages.MessageDownloading);
        }

        private void PreperatorOnPreperationInProgressEvent(object sender, PreperationProgressEventArgs preperationProgressEventArgs)
        {
            ProgressChanged(preperationProgressEventArgs.Percent, Messages.MessagePerperation);
        }

        private void RemoveHandler()
        {
            UpdaterService.Configuration.Provider.Preperator.PreperationInProgressEvent -= PreperatorOnPreperationInProgressEvent;
            UpdaterService.Configuration.Provider.Downloader.ProgressEvent -= DownloaderOnProgressEvent;
        }

        

        private void ProgressChanged(double percent, string message)
        {
            UpdateProgress = percent;
            UpdateText = message;
        }

        public event EventHandler LockUIEvent;
        public event EventHandler ShutdownEvent;

        public void Reset()
        {
            
        }

        public void Commit()
        {
            RemoveHandler();
        }

        public void Rollback()
        {
            RemoveHandler();
        }

        private void OnLockUIEvent()
        {
            LockUIEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnShutdownEvent()
        {
            ShutdownEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}