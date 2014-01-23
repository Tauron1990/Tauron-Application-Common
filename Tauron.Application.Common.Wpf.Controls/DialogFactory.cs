// The file DialogFactory.cs is part of Tauron.Application.Common.Wpf.Controls.
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
//  along with Tauron.Application.Common.Wpf.Controls If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The dialog factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Controls;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The dialog factory.</summary>
    [Export(typeof (IDialogFactory))]
    public class DialogFactory : IDialogFactory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create progress dialog.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="worker">
        ///     The worker.
        /// </param>
        /// <returns>
        ///     The <see cref="IProgressDialog" />.
        /// </returns>
        public IProgressDialog CreateProgressDialog(
            string text,
            string title,
            IWindow owner,
            Action<IProgress<ActiveProgress>> worker)
        {
            return new SimpleProgressDialog(text, title, owner, worker);
        }

        /// <summary>
        ///     The format exception.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="exception">
        ///     The exception.
        /// </param>
        public void FormatException(IWindow owner, Exception exception)
        {
            ShowMessageBox(
                owner,
                string.Format("Type: {0} \n {1}", exception.GetType(), exception.Message),
                "Error",
                MsgBoxButton.Ok,
                MsgBoxImage.Error,
                Properties.Resources.Erroricon);
        }

        /// <summary>
        ///     The get text.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="instruction">
        ///     The instruction.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="allowCancel">
        ///     The allow cancel.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetText(
            IWindow owner,
            string instruction,
            string content,
            string caption,
            bool allowCancel,
            string defaultValue)
        {
            return ObservableObject.CurrentDispatcher.Invoke(
                () =>
                {
                    Window realWindow = owner == null ? null : (Window) owner.TranslateForTechnology();
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

        /// <summary>
        ///     The show message box.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="button">
        ///     The button.
        /// </param>
        /// <param name="icon">
        ///     The icon.
        /// </param>
        /// <param name="custumIcon">
        ///     The custum icon.
        /// </param>
        /// <returns>
        ///     The <see cref="MsgBoxResult" />.
        /// </returns>
        public MsgBoxResult ShowMessageBox(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            Window realWindow = owner == null ? null : (Window)owner.TranslateForTechnology();

            return
                ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    !TaskDialog.OSSupportsTaskDialogs
                        ? (MsgBoxResult)
                          MessageBox.Show(
                              realWindow,
                              text,
                              caption,
                              (MessageBoxButton) button,
                              (MessageBoxImage) icon)
                        : ShowTaskDialog(owner, text, caption, button, icon,
                                         custumIcon));
        }

        /// <summary>
        ///     The show open file dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="checkFileExists">
        ///     The check file exists.
        /// </param>
        /// <param name="defaultExt">
        ///     The default ext.
        /// </param>
        /// <param name="dereferenceLinks">
        ///     The dereference links.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="multiSelect">
        ///     The multi select.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="validateNames">
        ///     The validate names.
        /// </param>
        /// <param name="checkPathExists">
        ///     The check path exists.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<string> ShowOpenFileDialog(
            IWindow owner,
            bool checkFileExists,
            string defaultExt,
            bool dereferenceLinks,
            string filter,
            bool multiSelect,
            string title,
            bool validateNames,
            bool checkPathExists,
            out bool? result)
        {
            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
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

                        tempresult = owner != null
                                         ? dialog.ShowDialog(
                                             (Window)
                                             owner
                                                 .TranslateForTechnology
                                                 ())
                                         : dialog.ShowDialog();

                        return tempresult == false
                                   ? null
                                   : dialog.FileNames;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        /// <summary>
        ///     The show open folder dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <param name="rootFolder">
        ///     The root folder.
        /// </param>
        /// <param name="showNewFolderButton">
        ///     The show new folder button.
        /// </param>
        /// <param name="useDescriptionForTitle">
        ///     The use description for title.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ShowOpenFolderDialog(
            IWindow owner,
            string description,
            Environment.SpecialFolder rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result)
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
                            RootFolder = rootFolder,
                            ShowNewFolderButton =
                                showNewFolderButton,
                            UseDescriptionForTitle =
                                useDescriptionForTitle
                        };

                        tempresult = owner != null
                                         ? dialog.ShowDialog(
                                             (Window)
                                             owner
                                                 .TranslateForTechnology
                                                 ())
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

        /// <summary>
        ///     The show save file dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="addExtension">
        ///     The add extension.
        /// </param>
        /// <param name="checkFileExists">
        ///     The check file exists.
        /// </param>
        /// <param name="checkPathExists">
        ///     The check path exists.
        /// </param>
        /// <param name="defaultExt">
        ///     The default ext.
        /// </param>
        /// <param name="dereferenceLinks">
        ///     The dereference links.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="createPrompt">
        ///     The create prompt.
        /// </param>
        /// <param name="overwritePrompt">
        ///     The overwrite prompt.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="initialDirectory">
        ///     The initial directory.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ShowSaveFileDialog(
            IWindow owner,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists,
            string defaultExt,
            bool dereferenceLinks,
            string filter,
            bool createPrompt,
            bool overwritePrompt,
            string title,
            string initialDirectory,
            out bool? result)
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
                            DereferenceLinks =
                                dereferenceLinks,
                            Filter = filter,
                            Title = title,
                            CheckPathExists = checkPathExists,
                            CreatePrompt = createPrompt,
                            OverwritePrompt = overwritePrompt,
                            InitialDirectory =
                                initialDirectory
                        };

                        tempresult = owner != null
                                         ? dialog.ShowDialog(
                                             (Window)
                                             owner
                                                 .TranslateForTechnology
                                                 ())
                                         : dialog.ShowDialog();

                        return tempresult == false ? null : dialog.FileName;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        /// <summary>
        ///     The show task dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="button">
        ///     The button.
        /// </param>
        /// <param name="icon">
        ///     The icon.
        /// </param>
        /// <param name="custumIcon">
        ///     The custum icon.
        /// </param>
        /// <returns>
        ///     The <see cref="MsgBoxResult" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public MsgBoxResult ShowTaskDialog(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            return ObservableObject.CurrentDispatcher.Invoke(
                () =>
                {
                    var dialog = new TaskDialog
                    {
                        CenterParent = true,
                        Content = text,
                        ExpandFooterArea = false,
                        ExpandedByDefault = false,
                        MinimizeBox = false,
                        ProgressBarStyle =
                            ProgressBarStyle.None,
                        WindowIcon = custumIcon,
                        WindowTitle = caption,
                        MainInstruction = caption,
                        MainIcon = TranslateIcon(icon)
                    };

                    TranslateButtons(button, dialog.Buttons);
                    TaskDialogButton clickedButton =
                        dialog.ShowDialog(owner != null
                                              ? (Window)
                                                owner
                                                    .TranslateForTechnology
                                                    ()
                                              : null);

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

        #endregion

        #region Methods

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
                    throw new ArgumentOutOfRangeException("button");
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
                    throw new ArgumentOutOfRangeException("icon");
            }
        }

        #endregion
    }
}