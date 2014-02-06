using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Addins
{
    [PublicAPI]
    public interface IAddInManager
    {
        /// <summary>
        /// Gets or sets the user addin path.
        /// This is the path where user AddIns are installed to.
        /// This property is normally initialized by <see cref="CoreStartup.ConfigureUserAddIns"/>.
        /// </summary>
        string UserAddInPath { get; set; }

        /// <summary>
        /// Gets or sets the addin install temporary directory.
        /// This is a directory used to store AddIns that should be installed on
        /// the next start of the application.
        /// This property is normally initialized by <see cref="CoreStartup.ConfigureUserAddIns"/>.
        /// </summary>
        string AddInInstallTemp { get; set; }

        /// <summary>
        /// Gets or sets the full name of the configuration file.
        /// In this file, the AddInManager stores the list of disabled AddIns
        /// and the list of installed external AddIns.
        /// This property is normally initialized by <see cref="CoreStartup.ConfigureExternalAddIns"/>.
        /// </summary>
        string ConfigurationFileName { get; set; }

        /// <summary>
        /// Installs the AddIns from AddInInstallTemp to the UserAddInPath.
        /// In case of installation errors, a error message is displayed to the user
        /// and the affected AddIn is added to the disabled list.
        /// This method is normally called by <see cref="CoreStartup.ConfigureUserAddIns"/>
        /// </summary>
        void InstallAddIns(List<string> disabled);

        /// <summary>
        /// Uninstalls the user addin on next start.
        /// <see cref="RemoveUserAddInOnNextStart"/> schedules the AddIn for
        /// deinstallation, you can unschedule it using
        /// <see cref="AbortRemoveUserAddInOnNextStart"/>
        /// </summary>
        /// <param name="identity">The identity of the addin to remove.</param>
        void RemoveUserAddInOnNextStart(string identity);

        /// <summary>
        /// Prevents a user AddIn from being uninstalled.
        /// <see cref="RemoveUserAddInOnNextStart"/> schedules the AddIn for
        /// deinstallation, you can unschedule it using
        /// <see cref="AbortRemoveUserAddInOnNextStart"/>
        /// </summary>
        /// <param name="identity">The identity of which to abort the removal.</param>
        void AbortRemoveUserAddInOnNextStart(string identity);

        /// <summary>
        /// Adds the specified external AddIns to the list of registered external
        /// AddIns.
        /// </summary>
        /// <param name="addIns">
        /// The list of AddIns to add. (use <see cref="AddIn"/> instances
        /// created by <see cref="AddIn.Load(IAddInTree,TextReader,string,XmlNameTable)"/>).
        /// </param>
        void AddExternalAddIns(IList<IAddIn> addIns);

        /// <summary>
        /// Removes the specified external AddIns from the list of registered external
        /// AddIns.
        /// </summary>
        /// The list of AddIns to remove.
        /// (use external AddIns from the <see cref="IAddInTree.AddIns"/> collection).
        void RemoveExternalAddIns(IList<IAddIn> addIns);

        /// <summary>
        /// Marks the specified AddIns as enabled (will take effect after
        /// next application restart).
        /// </summary>
        void Enable(IList<IAddIn> addIns);

        /// <summary>
        /// Marks the specified AddIns as disabled (will take effect after
        /// next application restart).
        /// </summary>
        void Disable(IList<AddIn> addIns);

        /// <summary>
        /// Loads a configuration file.
        /// The 'file' from XML elements in the form "&lt;AddIn file='full path to .addin file'&gt;" will
        /// be added to <paramref name="addInFiles"/>, the 'addin' element from
        /// "&lt;Disable addin='addin identity'&gt;" will be added to <paramref name="disabledAddIns"/>,
        /// all other XML elements are ignored.
        /// </summary>
        /// <param name="addInFiles">File names of external AddIns are added to this collection.</param>
        /// <param name="disabledAddIns">Identities of disabled addins are added to this collection.</param>
        void LoadAddInConfiguration(List<string> addInFiles, List<string> disabledAddIns);

        /// <summary>
        /// Saves the AddIn configuration in the format expected by
        /// <see cref="LoadAddInConfiguration"/>.
        /// </summary>
        /// <param name="addInFiles">List of file names of external AddIns.</param>
        /// <param name="disabledAddIns">List of Identities of disabled addins.</param>
        void SaveAddInConfiguration(List<string> addInFiles, List<string> disabledAddIns);
    }
}