#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Tauron.JetBrains.Annotations;

#endregion

namespace System.IO
{
    /// <summary>The io extensions.</summary>
    [PublicAPI]
    [DebuggerStepThrough]
    public static class IOExtensions
    {
        #region Public Methods and Operators

        [NotNull]
        public static string PathShorten([NotNull] this string path, int length)
        {
            string[] pathParts = path.Split('\\');
            var pathBuild = new StringBuilder(path.Length);
            string lastPart = pathParts[pathParts.Length - 1];
            string prevPath = "";

            //Erst prüfen ob der komplette String evtl. bereits kürzer als die Maximallänge ist
            if (path.Length >= length) return path;

            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                pathBuild.Append(pathParts[i] + @"\");
                if ((pathBuild + @"...\" + lastPart).Length >= length) return prevPath;
                prevPath = pathBuild + @"...\" + lastPart;
            }
            return prevPath;
        }

        /// <summary>
        ///     The clear.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void Clear(this DirectoryInfo dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");

            if (!dic.Exists) return;

            foreach (FileSystemInfo entry in dic.GetFileSystemInfos())
            {
                var file = entry as FileInfo;
                if (file != null) file.Delete();
                else
                {
                    var dici = entry as DirectoryInfo;
                    if (dici == null) continue;
                    
                    Clear(dici);
                    dici.Delete();
                }
            }
        }

        /// <summary>
        ///     The clear directory.
        /// </summary>+
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void ClearDirectory(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");

            Clear(new DirectoryInfo(dic));
        }

        /// <summary>
        ///     The clear parent directory.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void ClearParentDirectory(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");

            ClearDirectory(Path.GetDirectoryName(dic));
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="paths">
        ///     The paths.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CombinePath(this string path, params string[] paths)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(paths != null, "paths");
            Contract.Ensures(Contract.Result<string>() != null);

            paths = paths.Select(str => str.TrimStart('\\')).ToArray();

            if (Path.HasExtension(path)) path = Path.GetDirectoryName(path);

            string tempPath = Path.Combine(paths);
            return Path.Combine(path, tempPath);
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="path1">
        ///     The path 1.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CombinePath(this string path, string path1)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(path1 != null, "path1");
            Contract.Ensures(Contract.Result<string>() != null);

            if (Path.HasExtension(path)) path = Path.GetDirectoryName(path);

            return Path.Combine(path, path1);
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="path1">
        ///     The path 1.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CombinePath(this FileSystemInfo path, string path1)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(path1 != null, "path1");

            return CombinePath(path.FullName, path1);
        }

        /// <summary>
        ///     The copy file to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        public static void CopyFileTo(this string source, string destination)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source");
            Contract.Requires<ArgumentNullException>(destination != null, "destination");

            if (destination.ExisFile()) destination.DeleteFile();

            File.Copy(source, destination);
        }

        /// <summary>
        ///     The create directory if not exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CreateDirectoryIfNotExis(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (Path.HasExtension(path))
            {
                string temp = Path.GetDirectoryName(path);

                return CreateDirectoryIfNotExis(new DirectoryInfo(temp));
            }

            return CreateDirectoryIfNotExis(new DirectoryInfo(path));
        }

        /// <summary>
        ///     The create directory if not exis.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CreateDirectoryIfNotExis(this DirectoryInfo dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");

            if (dic.Exists) return false;

            dic.Create();

            return true;
        }

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        public static void Delete(this FileSystemInfo info)
        {
            Contract.Requires<ArgumentNullException>(info != null, "info");

            if (info.Exists) info.Delete();
        }

        /// <summary>
        ///     The delete directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void DeleteDirectory(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
                Contract.Assert(path != null);
            }

            try
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        ///     The delete directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="sub">
        ///     The sub.
        /// </param>
        public static void DeleteDirectory(this string path, object sub)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(sub != null, "sub");

            string compl = CombinePath(path, sub.ToString());
            if (Directory.Exists(compl)) Directory.Delete(compl);
        }

        public static void DeleteDirectory(this string path, bool recursive)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (Directory.Exists(path)) Directory.Delete(path, recursive);
        }

        /// <summary>
        ///     The delete directory if empty.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        [ContractVerification(false)]
        public static void DeleteDirectoryIfEmpty(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (!Directory.EnumerateFileSystemEntries(path).GetEnumerator().MoveNext()) Directory.Delete(path);
        }

        /// <summary>
        ///     The delete file.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void DeleteFile(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (!path.ExisFile()) return;

            File.Delete(path);
        }

        public static bool DirectoryConainsInvalidChars(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            char[] invalid = Path.GetInvalidPathChars();

            return path.All(invalid.Contains);
        }

        public static IEnumerable<string> EnumrateFileSystemEntries(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return Directory.EnumerateFileSystemEntries(dic);
        }

        public static IEnumerable<string> EnumerateAllFiles(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return Directory.EnumerateFiles(dic, "*.*", SearchOption.AllDirectories);
        }

        public static IEnumerable<string> EnumerateAllFiles(this string dic, string filter)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Requires<ArgumentNullException>(filter != null, "filter");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return Directory.EnumerateFiles(dic, filter, SearchOption.AllDirectories);
        }

        /// <summary>
        ///     The enumerate directorys.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [ContractVerification(false)]
        public static IEnumerable<string> EnumerateDirectorys(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            if (!Directory.Exists(path)) return Enumerable.Empty<string>();

            return Directory.EnumerateDirectories(path);
        }

        /// <summary>
        ///     The enumerate file system entrys.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<FileSystemInfo> EnumerateFileSystemEntrys(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Ensures(Contract.Result<IEnumerable<FileSystemInfo>>() != null);

            return new DirectoryInfo(path).EnumerateFileSystemInfos();
        }

        /// <summary>
        ///     The enumerate all files.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<string> EnumerateFiles(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return Directory.EnumerateFiles(dic, "*.*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(this string dic, string filter)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");
            Contract.Requires<ArgumentNullException>(filter != null, "filter");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return Directory.EnumerateFiles(dic, filter, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        ///     The enumerate text lines if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<string> EnumerateTextLinesIfExis(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (!File.Exists(path)) yield break;

            using (StreamReader reader = File.OpenText(path))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) yield break;

                    yield return line;
                }
            }
        }

        public static IEnumerable<string> EnumerateTextLines(this TextReader reader)
        {
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }

        /// <summary>
        ///     The exis directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisDirectory(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Directory.Exists(path);
        }

        /// <summary>
        ///     The exis file.
        /// </summary>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="file">
        ///     The file.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisFile(this string workingDirectory, string file)
        {
            Contract.Requires<ArgumentNullException>(workingDirectory != null, "workingDirectory");
            Contract.Requires<ArgumentNullException>(file != null, "file");

            try
            {
                return File.Exists(Path.Combine(workingDirectory, file));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        ///     The exis file.
        /// </summary>
        /// <param name="file">
        ///     The file.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisFile(this string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return false;

            return File.Exists(file);
        }

        /// <summary>
        ///     The get directory creation time.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime GetDirectoryCreationTime(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Directory.GetCreationTime(path);
        }

        /// <summary>
        ///     The get directory name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetDirectoryName(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        ///     The get directory name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetDirectoryName(this StringBuilder path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return GetDirectoryName(path.ToString());
        }

        /// <summary>
        ///     The get directorys.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        public static string[] GetDirectorys(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Directory.GetDirectories(path);
        }

        /// <summary>
        ///     The get extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetExtension(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.GetExtension(path);
        }

        /// <summary>
        ///     The get file name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetFileName(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.GetFileName(path);
        }

        /// <summary>
        ///     The get file name without extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetFileNameWithoutExtension(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        ///     The get file system count.
        /// </summary>
        /// <param name="strDir">
        ///     The str dir.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetFileSystemCount(this string strDir)
        {
            Contract.Requires<ArgumentNullException>(strDir != null, "strDir");

            // 0. Einstieg in die Rekursion auf oberster Ebene
            return GetFileSystemCount(new DirectoryInfo(strDir));
        }

        /// <summary>
        ///     The get file system count.
        /// </summary>
        /// <param name="di">
        ///     The di.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetFileSystemCount(this DirectoryInfo di)
        {
            Contract.Requires<ArgumentNullException>(di != null, "di");

            int count = 0;

            try
            {
                // 1. Für alle Dateien im aktuellen Verzeichnis
                count += di.GetFiles().Count();

                // 2. Für alle Unterverzeichnisse im aktuellen Verzeichnis
                foreach (DirectoryInfo diSub in di.GetDirectories())
                {
                    // 2a. Statt Console.WriteLine hier die gewünschte Aktion
                    count++;

                    // 2b. Rekursiver Abstieg
                    count += GetFileSystemCount(diSub);
                }
            }
            catch (Exception e)
            {
                // 3. Statt Console.WriteLine hier die gewünschte Aktion
                if (ExceptionPolicy.HandleException(e, "General")) throw;
            }

            return count;
        }

        /// <summary>
        ///     The get files.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        public static string[] GetFiles(this string dic)
        {
            Contract.Requires<ArgumentNullException>(dic != null, "dic");

            return Directory.GetFiles(dic);
        }

        /// <summary>
        ///     The get files.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="option">
        ///     The option.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        public static string[] GetFiles(this string path, string pattern, SearchOption option)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(pattern != null, "pattern");

            return Directory.GetFiles(path, pattern, option);
        }

        /// <summary>
        ///     The get full path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetFullPath(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.GetFullPath(path);
        }

        /// <summary>
        ///     The has extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasExtension(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.HasExtension(path);
        }

        /// <summary>
        ///     The is path rooted.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsPathRooted(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return Path.IsPathRooted(path);
        }

        /// <summary>
        ///     The move to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="dest">
        ///     The dest.
        /// </param>
        public static void MoveTo(this string source, string dest)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source");
            Contract.Requires<ArgumentNullException>(dest != null, "dest");
            Contract.Requires<ArgumentException>(source.Length != 0, "source");
            Contract.Requires<ArgumentException>(dest.Length != 0, "dest");

            File.Move(source, dest);
        }

        /// <summary>
        ///     The move to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="dest">
        ///     The dest.
        /// </param>
        public static void MoveTo(this string source, string workingDirectory, string dest)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(source), "source");
            Contract.Requires<ArgumentNullException>(workingDirectory != null, "workingDirectory");
            Contract.Requires<ArgumentNullException>(dest != null, "dest");

            string realDest = dest;

            if (!dest.HasExtension())
            {
                string fileName = Path.GetFileName(source);
                realDest = Path.Combine(dest, fileName);
            }

            string realSource = Path.Combine(workingDirectory, source);

            Contract.Assume(realDest.Length != 0);
            Contract.Assume(realSource.Length != 0);

            File.Move(realSource, realDest);
        }

        /// <summary>
        ///     The open read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="share">
        ///     The share.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        public static Stream OpenRead(this string path, FileShare share)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, share);
        }

        /// <summary>
        ///     The open read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        public static Stream OpenRead(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return OpenRead(path, FileShare.None);
        }

        /// <summary>
        ///     The open text append.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamWriter" />.
        /// </returns>
        public static StreamWriter OpenTextAppend(this string path)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(path), "path");

            path.CreateDirectoryIfNotExis();
            return new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None));
        }

        /// <summary>
        ///     The open text read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamReader" />.
        /// </returns>
        public static StreamReader OpenTextRead(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return File.OpenText(path);
        }

        /// <summary>
        ///     The open text write.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamWriter" />.
        /// </returns>
        public static StreamWriter OpenTextWrite(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            path.CreateDirectoryIfNotExis();
            return new StreamWriter(path);
        }

        /// <summary>
        ///     The open write.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        public static Stream OpenWrite(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return OpenWrite(path, FileShare.None);
        }

        /// <summary>
        ///     The open write.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="share">
        ///     The share.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        public static Stream OpenWrite(this string path, FileShare share)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, share);
        }

        public static byte[] ReadAllBytesIfExis(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Ensures(Contract.Result<byte[]>() != null);

            if (!File.Exists(path)) return new byte[0];

            return File.ReadAllBytes(path);
        }

        /// <summary>
        ///     The read text if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string ReadTextIfExis(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        /// <summary>
        ///     The read text if exis.
        /// </summary>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="subPath">
        ///     The sub path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string ReadTextIfExis(this string workingDirectory, string subPath)
        {
            Contract.Requires<ArgumentNullException>(workingDirectory != null, "workingDirectory");
            Contract.Requires<ArgumentNullException>(subPath != null, "subPath");

            return ReadTextIfExis(CombinePath(workingDirectory, subPath));
        }

        /// <summary>
        ///     The read text lines if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        public static IEnumerable<string> ReadTextLinesIfExis(this string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            if (!File.Exists(path)) yield break;

            using (StreamReader reader = File.OpenText(path))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    yield return line;
                }
            }
        }

        /// <summary>
        ///     The create uri without scheme.
        /// </summary>
        /// <param name="str">
        ///     The str.
        /// </param>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="scheme">
        ///     The scheme.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool TryCreateUriWithoutScheme(this string str, out Uri uri, params string[] scheme)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(str), "enumerator");
            Contract.Requires<ArgumentNullException>(scheme != null, "scheme");

            Uri target;
            bool flag = Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out target);

            if (flag) foreach (string s in scheme) if (flag) flag = target.Scheme != s;

            uri = flag ? target : null;

            return flag;
        }

        /// <summary>
        ///     The write text content to.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void WriteTextContentTo(this string content, string path)
        {
            Contract.Requires<ArgumentNullException>(content != null, "content");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            File.WriteAllText(path, content);
        }

        /// <summary>
        ///     The write text content to.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void WriteTextContentTo(this string content, string workingDirectory, string path)
        {
            Contract.Requires<ArgumentNullException>(content != null, "content");
            Contract.Requires<ArgumentNullException>(workingDirectory != null, "workingDirectory");
            Contract.Requires<ArgumentNullException>(path != null, "path");

            WriteTextContentTo(content, CombinePath(workingDirectory, path));
        }

        #endregion
    }
}