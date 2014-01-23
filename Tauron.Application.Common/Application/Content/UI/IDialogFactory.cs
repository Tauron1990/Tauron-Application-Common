// The file IDialogFactory.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The msg box image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The msg box image.</summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    [PublicAPI]
    public enum MsgBoxImage
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The error.</summary>
        Error = 16,

        /// <summary>The hand.</summary>
        Hand = 16,

        /// <summary>The stop.</summary>
        Stop = 16,

        /// <summary>The question.</summary>
        Question = 32,

        /// <summary>The exclamation.</summary>
        Exclamation = 48,

        /// <summary>The warning.</summary>
        Warning = 48,

        /// <summary>The asterisk.</summary>
        Asterisk = 64,

        /// <summary>The information.</summary>
        Information = 64,
    }

    /// <summary>The msg box button.</summary>
    [PublicAPI]
    public enum MsgBoxButton
    {
        /// <summary>The ok.</summary>
        Ok = 0,

        /// <summary>The ok cancel.</summary>
        OkCancel = 1,

        /// <summary>The yes no cancel.</summary>
        YesNoCancel = 3,

        /// <summary>The yes no.</summary>
        YesNo = 4,
    }

    /// <summary>The msg box result.</summary>
    [PublicAPI]
    public enum MsgBoxResult
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The ok.</summary>
        Ok = 1,

        /// <summary>The cancel.</summary>
        Cancel = 2,

        /// <summary>The yes.</summary>
        Yes = 6,

        /// <summary>The no.</summary>
        No = 7,
    }

    /// <summary>The DialogFactory interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (DialogFactoryContracts))]
    public interface IDialogFactory
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
        [NotNull,SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [PublicAPI]
        IProgressDialog CreateProgressDialog([NotNull] string text, [NotNull] string title, [NotNull] IWindow owner, [NotNull] Action<IProgress<ActiveProgress>> worker);

        /// <summary>
        ///     The format exception.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="exception">
        ///     The exception.
        /// </param>
        [PublicAPI]
        void FormatException([CanBeNull] IWindow owner, [NotNull] Exception exception);

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
        [NotNull,PublicAPI]
        string GetText([CanBeNull] IWindow owner, [NotNull] string instruction, [CanBeNull] string content, [NotNull] string caption,
            bool allowCancel, [CanBeNull] string defaultValue);

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
        [PublicAPI]
        MsgBoxResult ShowMessageBox([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
            MsgBoxButton button,
            MsgBoxImage icon, [CanBeNull] Icon custumIcon);

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
        [NotNull]
        IEnumerable<string> ShowOpenFileDialog([CanBeNull] IWindow owner,
            bool checkFileExists, [NotNull] string defaultExt,
            bool dereferenceLinks, [NotNull] string filter,
            bool multiSelect, [NotNull] string title,
            bool validateNames,
            bool checkPathExists,
            out bool? result);

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
        [NotNull]
        string ShowOpenFolderDialog([CanBeNull] IWindow owner, [NotNull] string description,
            Environment.SpecialFolder rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result);

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
        [NotNull]
        string ShowSaveFileDialog([CanBeNull] IWindow owner,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists, [NotNull] string defaultExt,
            bool dereferenceLinks, [NotNull] string filter,
            bool createPrompt,
            bool overwritePrompt, [NotNull] string title, [NotNull] string initialDirectory,
            out bool? result);

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
        [PublicAPI]
        MsgBoxResult ShowTaskDialog([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
            MsgBoxButton button,
            MsgBoxImage icon, [CanBeNull] Icon custumIcon);

        #endregion
    }

    [ContractClassFor(typeof (IDialogFactory))]
    internal abstract class DialogFactoryContracts : IDialogFactory
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IProgressDialog CreateProgressDialog(
            string text,
            string title,
            IWindow owner,
            Action<IProgress<ActiveProgress>> worker)
        {
            Contract.Requires<ArgumentNullException>(text != null, "text");
            Contract.Requires<ArgumentNullException>(title != null, "title");
            Contract.Requires<ArgumentNullException>(worker != null, "worker");
            Contract.Ensures(Contract.Result<IProgressDialog>() != null);

            throw new NotImplementedException();
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
            Contract.Requires<ArgumentNullException>(exception != null, "exception");
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string GetText(
            IWindow owner,
            string instruction,
            string content,
            string caption,
            bool allowCancel,
            string defaultValue)
        {
            Contract.Requires<ArgumentNullException>(instruction != null, "instruction");
            Contract.Requires<ArgumentNullException>(content != null, "content");
            Contract.Requires<ArgumentNullException>(caption != null, "caption");

            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public MsgBoxResult ShowMessageBox(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            Contract.Requires<ArgumentNullException>(text != null, "text");
            Contract.Requires<ArgumentNullException>(caption != null, "caption");

            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// </exception>
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
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string ShowOpenFolderDialog(
            IWindow owner,
            string description,
            Environment.SpecialFolder rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// </exception>
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
            throw new NotImplementedException();
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
        public MsgBoxResult ShowTaskDialog(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            Contract.Requires<ArgumentNullException>(text != null, "text");
            Contract.Requires<ArgumentNullException>(caption != null, "caption");
            return MsgBoxResult.Cancel;
        }

        #endregion
    }
}