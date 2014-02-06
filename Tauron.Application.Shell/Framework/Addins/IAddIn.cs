using System;
using System.Collections.Generic;
using System.IO;

namespace Tauron.Application.Shell.Framework.Addins
{
    public interface IAddIn
    {
        /// <summary>
        /// Gets the parent AddIn-Tree that contains this AddIn.
        /// </summary>
        IAddInTree AddInTree { get; }

        /// <summary>
        /// Gets the message of a custom load error. Used only when AddInAction is set to CustomError.
        /// Settings this property to a non-null value causes Enabled to be set to false and
        /// Action to be set to AddInAction.CustomError.
        /// </summary>
        string CustomErrorMessage { get; }

        /// <summary>
        /// Action to execute when the application is restarted.
        /// </summary>
        AddInAction Action { get; set; }

        IReadOnlyList<Runtime> Runtimes { get; }
        Version Version { get; }
        string FileName { get; set; }
        string Name { get; }
        AddInManifest Manifest { get; }
        Dictionary<string, ExtensionPath> Paths { get; }
        Properties Properties { get; }
        List<string> BitmapResources { get; set; }
        List<string> StringResources { get; set; }
        bool Enabled { get; set; }

        /// <summary>
        /// Gets whether the AddIn is a preinstalled component of the host application.
        /// </summary>
        bool IsPreinstalled { get; }

        object CreateObject(string className);
        Type FindType(string className);
        Stream GetManifestResourceStream(string resourceName);
        void LoadRuntimeAssemblies();
        string ToString();
        ExtensionPath GetExtensionPath(string pathName);
    }
}