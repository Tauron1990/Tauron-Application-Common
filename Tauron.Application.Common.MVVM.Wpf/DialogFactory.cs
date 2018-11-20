using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Controls;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [Export(typeof(IDialogFactory)), DebuggerStepThrough]
    public class DialogFactory : IDialogFactory
    {
        public IProgressDialog CreateProgressDialog(string text, string title, IWindow owner, Action<IProgress<ActiveProgress>> worker) 
            => new SimpleProgressDialog(text, title, Argument.NotNull(owner, nameof(owner)), worker);

        public void FormatException(IWindow owner, Exception exception) 
            => ShowMessageBox(owner, $"Type: {exception.GetType().Name} \n {exception.Message}", "Error", MsgBoxButton.Ok, MsgBoxImage.Error);

        public string GetText(IWindow owner, string instruction, string content, string caption, bool allowCancel, string defaultValue)
        {
            return ObservableObject.CurrentDispatcher.Invoke(() =>
            {
                var realWindow = (Window) owner?.TranslateForTechnology();
                var diag = new InputDialog
                {
                    Owner = realWindow,
                    MainText = instruction,
                    AllowCancel = allowCancel,
                    Title = caption,
                    InstructionText = content,
                    Result = defaultValue
                };

                return diag.ShowDialog() == true ? diag.Result : null;
            });
        }


        public MsgBoxResult ShowMessageBox(IWindow owner, string text, string caption, MsgBoxButton button, MsgBoxImage icon)
        {
            var realWindow = owner?.TranslateForTechnology() as Window;

            return
                ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                        !TaskDialog.OSSupportsTaskDialogs
                            ? (MsgBoxResult)
                            MessageBox.Show(
                                Argument.NotNull(realWindow, nameof(owner), "No WPF Window or Window Null"),
                                text,
                                caption,
                                (MessageBoxButton) button,
                                (MessageBoxImage) icon)
                            : ShowTaskDialog(owner, text, caption, button, icon));
        }


        public IEnumerable<string> ShowOpenFileDialog(IWindow owner, bool checkFileExists, string defaultExt, bool dereferenceLinks, string filter,
            bool multiSelect, string title, bool validateNames, bool checkPathExists, out bool? result)
        {
            Argument.NotNull(owner, nameof(owner));

            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(() =>
                    {
                        var dialog = new VistaOpenFileDialog
                        {
                            CheckFileExists = checkFileExists,
                            DefaultExt = defaultExt,
                            DereferenceLinks =
                                dereferenceLinks,
                            Filter = filter,
                            Multiselect = multiSelect,
                            Title = title,
                            ValidateNames = validateNames,
                            CheckPathExists = checkPathExists
                        };

                        TranslateDefaultExt(dialog);

                        tempresult = owner != null
                            ? dialog.ShowDialog((Window)owner.TranslateForTechnology())
                            : dialog.ShowDialog();

                        return tempresult == false
                            ? Enumerable.Empty<string>()
                            : dialog.FileNames;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public string ShowOpenFolderDialog(IWindow owner, string description, Environment.SpecialFolder rootFolder, bool showNewFolderButton, bool useDescriptionForTitle, out bool? result)
        {
            Argument.NotNull(owner, nameof(owner));

            bool? tempresult = null;
            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaFolderBrowserDialog
                        {
                            Description = description,
                            RootFolder = rootFolder,
                            ShowNewFolderButton = showNewFolderButton,
                            UseDescriptionForTitle = useDescriptionForTitle
                        };

                        tempresult = owner != null
                            ? dialog.ShowDialog((Window)owner.TranslateForTechnology())
                            : dialog.ShowDialog();

                        return tempresult == false
                            ? null
                            : dialog.SelectedPath;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public string ShowOpenFolderDialog(IWindow owner, string description, string rootFolder, bool showNewFolderButton, bool useDescriptionForTitle, out bool? result)
        {
            bool? tempresult = null;
            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaFolderBrowserDialog
                        {
                            Description = description,
                            SelectedPath = rootFolder,
                            ShowNewFolderButton = showNewFolderButton,
                            UseDescriptionForTitle = useDescriptionForTitle
                        };

                        tempresult = owner != null
                            ? dialog.ShowDialog((Window)owner.TranslateForTechnology())
                            : dialog.ShowDialog();

                        return tempresult == false
                            ? null
                            : dialog.SelectedPath;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public string ShowSaveFileDialog(IWindow owner, bool addExtension, bool checkFileExists, bool checkPathExists, string defaultExt, bool dereferenceLinks, string filter,
            bool createPrompt, bool overwritePrompt, string title, string initialDirectory, out bool? result)
        {
            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaSaveFileDialog
                        {
                            AddExtension = addExtension,
                            CheckFileExists = checkFileExists,
                            DefaultExt = defaultExt,
                            DereferenceLinks = dereferenceLinks,
                            Filter = filter,
                            Title = title,
                            CheckPathExists = checkPathExists,
                            CreatePrompt = createPrompt,
                            OverwritePrompt = overwritePrompt,
                            InitialDirectory = initialDirectory
                        };

                        TranslateDefaultExt(dialog);

                        tempresult = owner != null
                            ? dialog.ShowDialog((Window)owner.TranslateForTechnology())
                            : dialog.ShowDialog();

                        return tempresult == false ? null : dialog.FileName;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public MsgBoxResult ShowTaskDialog(IWindow owner, string text, string caption, MsgBoxButton button, MsgBoxImage icon)
        {
            return ObservableObject.CurrentDispatcher.Invoke(() =>
                {
                    var dialog = new TaskDialog
                    {
                        CenterParent = true,
                        Content = text,
                        ExpandFooterArea = false,
                        ExpandedByDefault = false,
                        MinimizeBox = false,
                        ProgressBarStyle = ProgressBarStyle.None,
                        WindowTitle = caption,
                        MainInstruction = caption,
                        MainIcon = TranslateIcon(icon)
                    };

                    TranslateButtons(button, dialog.Buttons);
                    var clickedButton =
                        dialog.ShowDialog((Window) owner?.TranslateForTechnology());

                    switch (clickedButton.ButtonType)
                    {
                        case ButtonType.Ok:
                            return MsgBoxResult.Ok;
                        case ButtonType.Yes:
                            return MsgBoxResult.Yes;
                        case ButtonType.No:
                            return MsgBoxResult.No;
                        case ButtonType.Cancel:
                            return MsgBoxResult.Cancel;
                        case ButtonType.Close:
                            return MsgBoxResult.Cancel;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }
        
        private void TranslateDefaultExt([NotNull] VistaFileDialog dialog)
        {
            if (string.IsNullOrWhiteSpace(dialog.DefaultExt)) return;

            var ext = "*." + dialog.DefaultExt;
            var filter = dialog.Filter;
            var filters = filter.Split('|');
            for (var i = 1; i < filters.Length; i += 2)
                if (filters[i] == ext)
                    dialog.FilterIndex = 1 + (i - 1) / 2;
        }

        private void TranslateButtons(MsgBoxButton button, [NotNull] TaskDialogItemCollection<TaskDialogButton> buttons)
        {
            switch (button)
            {
                case MsgBoxButton.OkCancel:
                    buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    buttons.Add(new TaskDialogButton(ButtonType.Cancel));
                    break;
                case MsgBoxButton.Ok:
                    buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    break;
                case MsgBoxButton.YesNoCancel:
                    buttons.Add(new TaskDialogButton(ButtonType.Yes));
                    buttons.Add(new TaskDialogButton(ButtonType.No));
                    buttons.Add(new TaskDialogButton(ButtonType.Cancel));
                    break;
                case MsgBoxButton.YesNo:
                    buttons.Add(new TaskDialogButton(ButtonType.Yes));
                    buttons.Add(new TaskDialogButton(ButtonType.No));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button));
            }
        }

        private TaskDialogIcon TranslateIcon(MsgBoxImage icon)
        {
            switch (icon)
            {
                case MsgBoxImage.None:
                    return TaskDialogIcon.Custom;
                case MsgBoxImage.Error:
                    return TaskDialogIcon.Error;
                case MsgBoxImage.Question:
                    return TaskDialogIcon.Shield;
                case MsgBoxImage.Warning:
                    return TaskDialogIcon.Warning;
                case MsgBoxImage.Information:
                    return TaskDialogIcon.Information;
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon));
            }
        }
    }
}