using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NLog;

namespace Tauron
{
    [PublicAPI]
    public static class IOExtensions
    {
        [NotNull]
        public static string PathShorten([NotNull] this string path, int length)
        {
            var pathParts = path.Split('\\');
            var pathBuild = new StringBuilder(path.Length);
            var lastPart = pathParts[pathParts.Length - 1];
            var prevPath = "";

            //Erst prüfen ob der komplette String evtl. bereits kürzer als die Maximallänge ist
            if (path.Length >= length) return path;

            for (var i = 0; i < pathParts.Length - 1; i++)
            {
                pathBuild.Append(pathParts[i] + @"\");
                if ((pathBuild + @"...\" + lastPart).Length >= length) return prevPath;
                prevPath = pathBuild + @"...\" + lastPart;
            }

            return prevPath;
        }


        public static void Clear([NotNull] this DirectoryInfo dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (!dic.Exists) return;

            foreach (var entry in dic.GetFileSystemInfos())
            {
                if (entry is FileInfo file)
                    file.Delete();
                else
                {
                    if (!(entry is DirectoryInfo dici)) continue;

                    Clear(dici);
                    dici.Delete();
                }
            }
        }

        public static void ClearDirectory([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            Clear(new DirectoryInfo(dic));
        }


        public static void ClearParentDirectory([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            ClearDirectory(Argument.CheckResult(Path.GetDirectoryName(dic), "Result Path Was null"));
        }


        [NotNull]
        public static string CombinePath([NotNull] this string path, [ItemNotNull] [NotNull] params string[] paths)
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(paths, nameof(paths));

            paths = paths.Select(str => str.TrimStart('\\')).ToArray();
            if (Path.HasExtension(path)) path = Argument.CheckResult(Path.GetDirectoryName(path), "Result Path Was null");

            var tempPath = Path.Combine(paths);
            return Path.Combine(path, tempPath);
        }


        [NotNull]
        public static string CombinePath([NotNull] this string path, [NotNull] string path1)
        {
            Argument.NotNull(path1, nameof(path1));
            Argument.NotNull((object)path, nameof(path));
            return string.IsNullOrWhiteSpace(path) ? path1 : Path.Combine(path, path1);
        }

        [NotNull]
        public static string CombinePath([NotNull] this FileSystemInfo path, [NotNull] string path1)
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(path1, nameof(path1));
            return CombinePath(path.FullName, path1);
        }

        public static void CopyFileTo([NotNull] this string source, [NotNull] string destination)
        {
            Argument.NotNull(source, nameof(source));
            Argument.NotNull(destination, nameof(destination));

            if (!source.ExisFile()) return;

            File.Copy(source, destination, true);
        }

        public static bool CreateDirectoryIfNotExis([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            try
            {
                if (!Path.HasExtension(path)) return CreateDirectoryIfNotExis(new DirectoryInfo(path));

                var temp = Path.GetDirectoryName(path);

                return CreateDirectoryIfNotExis(new DirectoryInfo(temp ?? throw new InvalidOperationException()));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool CreateDirectoryIfNotExis([NotNull] this DirectoryInfo dic)
        {
            Argument.NotNull(dic, nameof(dic));
            if (dic.Exists) return false;
            dic.Create();

            return true;
        }

        public static void SafeDelete([NotNull] this FileSystemInfo info)
        {
            Argument.NotNull(info, nameof(info));
            if (info.Exists) info.Delete();
        }

        public static void DeleteDirectory([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));

            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            try
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public static void DeleteDirectory([NotNull] this string path, [NotNull] object sub)
        {
            Argument.NotNull(path, nameof(path));
            var tempsub = sub?.ToString();
            Argument.NotNull(tempsub, nameof(sub));

            var compl = CombinePath(path, tempsub);
            if (Directory.Exists(compl)) Directory.Delete(compl);
        }

        public static void DeleteDirectory([NotNull] this string path, bool recursive)
        {
            Argument.NotNull(path, nameof(path));
            if (Directory.Exists(path)) Directory.Delete(path, recursive);
        }

        public static void DeleteDirectoryIfEmpty([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            if(!Directory.Exists(path)) return;
            if (!Directory.EnumerateFileSystemEntries(path).Any()) Directory.Delete(path);
        }

        public static void DeleteFile([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            if (!path.ExisFile()) return;

            File.Delete(path);
        }

        public static bool DirectoryConainsInvalidChars([NotNull] this string path)
        {
            var invalid = Path.GetInvalidPathChars();
            return path?.Any(invalid.Contains) ?? true;
        }

        [NotNull]
        public static IEnumerable<string> EnumrateFileSystemEntries([NotNull] this string dic)
        {
            Argument.NotNull(dic, nameof(dic));
            return Directory.EnumerateFileSystemEntries(dic);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateAllFiles([NotNull] this string dic)
        {
            Argument.NotNull(dic, nameof(dic));
            return Directory.EnumerateFiles(dic, "*.*", SearchOption.AllDirectories);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateAllFiles([NotNull] this string dic, [NotNull] string filter)
        {
            Argument.NotNull(dic, nameof(dic));
            Argument.NotNull(filter, nameof(filter));
            return Directory.EnumerateFiles(dic, filter, SearchOption.AllDirectories);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateDirectorys([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateDirectories(path);
        }

        [NotNull]
        public static IEnumerable<FileSystemInfo> EnumerateFileSystemEntrys([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return new DirectoryInfo(path).EnumerateFileSystemInfos();
        }

        [NotNull]
        public static IEnumerable<string> EnumerateFiles([NotNull] this string dic)
        {
            Argument.NotNull(dic, nameof(dic));
            return Directory.EnumerateFiles(dic, "*.*", SearchOption.TopDirectoryOnly);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateFiles([NotNull] this string dic, [NotNull] string filter)
        {
            Argument.NotNull(dic, nameof(dic));
            Argument.NotNull(filter, nameof(filter));
            return Directory.EnumerateFiles(dic, filter, SearchOption.TopDirectoryOnly);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateTextLinesIfExis([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            if (!File.Exists(path)) yield break;

            using (var reader = File.OpenText(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) yield break;

                    yield return line;
                }
            }
        }

        [NotNull]
        public static IEnumerable<string> EnumerateTextLines([NotNull] this TextReader reader)
        {
            Argument.NotNull(reader, nameof(reader));
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }

        public static bool ExisDirectory([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Directory.Exists(path);
        }

        public static bool ExisFile([NotNull] this string workingDirectory, [NotNull] string file)
        {
            Argument.NotNull(workingDirectory, nameof(workingDirectory));
            Argument.NotNull(file, nameof(file));
            try
            {
                return File.Exists(Path.Combine(workingDirectory, file));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool ExisFile([NotNull] this string file) => !string.IsNullOrWhiteSpace(file) && File.Exists(file);

        public static DateTime GetDirectoryCreationTime([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Directory.GetCreationTime(path);
        }

        [NotNull]
        public static string GetDirectoryName([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Argument.CheckResult(Path.GetDirectoryName(path), "Result Path was Null");
        }

        [NotNull]
        public static string GetDirectoryName([NotNull] this StringBuilder path)
        {
            Argument.NotNull(path, nameof(path));
            return GetDirectoryName(path.ToString());
        }


        [NotNull]
        public static string[] GetDirectorys([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Directory.GetDirectories(path);
        }

        [NotNull]
        public static string GetExtension([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.GetExtension(path);
        }

        [NotNull]
        public static string GetFileName([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.GetFileName(path);
        }

        [NotNull]
        public static string GetFileNameWithoutExtension([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.GetFileNameWithoutExtension(path);
        }

        public static int GetFileSystemCount([NotNull] this string strDir)
        {
            Argument.NotNull(strDir, nameof(strDir));
            // 0. Einstieg in die Rekursion auf oberster Ebene
            return GetFileSystemCount(new DirectoryInfo(strDir));
        }

        public static int GetFileSystemCount([NotNull] this DirectoryInfo di)
        {
            Argument.NotNull(di, nameof(di));
            var count = 0;

            try
            {
                // 1. Für alle Dateien im aktuellen Verzeichnis
                count += di.GetFiles().Length;

                // 2. Für alle Unterverzeichnisse im aktuellen Verzeichnis
                foreach (var diSub in di.GetDirectories())
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
                LogManager.GetLogger(nameof(IOExtensions), typeof(IOExtensions)).Error(e);
                throw;
            }

            return count;
        }

        [NotNull]
        public static string[] GetFiles([NotNull] this string dic)
        {
            Argument.NotNull(dic, nameof(dic));
            return Directory.GetFiles(dic);
        }

        [NotNull]
        public static string[] GetFiles([NotNull] this string path, [NotNull] string pattern, SearchOption option)
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(pattern, nameof(pattern));
            return Directory.GetFiles(path, pattern, option);
        }

        [NotNull]
        public static string GetFullPath([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.GetFullPath(path);
        }

        public static bool HasExtension([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.HasExtension(path);
        }

        public static bool IsPathRooted([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return Path.IsPathRooted(path);
        }

        public static void MoveTo([NotNull] this string source, [NotNull] string dest)
        {
            Argument.NotNull(source, nameof(source));
            Argument.NotNull(dest, nameof(dest));
            File.Move(source, dest);
        }

        public static void MoveTo([NotNull] this string source, [NotNull] string workingDirectory, [NotNull] string dest)
        {
            Argument.NotNull(source, nameof(source));
            Argument.NotNull(workingDirectory, nameof(workingDirectory));
            Argument.NotNull(dest, nameof(dest));
            var realDest = dest;

            if (!dest.HasExtension())
            {
                var fileName = Path.GetFileName(source);
                realDest = Path.Combine(dest, fileName);
            }

            var realSource = Path.Combine(workingDirectory, source);

            File.Move(realSource, realDest);
        }

        [NotNull]
        public static Stream OpenRead([NotNull] this string path, FileShare share)
        {
            Argument.NotNull(path, nameof(path));
            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, share);
        }

        [NotNull]
        public static Stream OpenRead([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return OpenRead(path, FileShare.None);
        }

        [NotNull]
        public static StreamWriter OpenTextAppend([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None));
        }

        [NotNull]
        public static StreamReader OpenTextRead([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return File.OpenText(path);
        }

        [NotNull]
        public static StreamWriter OpenTextWrite([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(path);
        }

        [NotNull]
        public static Stream OpenWrite([NotNull] this string path, bool delete = true)
        {
            Argument.NotNull(path, nameof(path));
            return OpenWrite(path, FileShare.None, delete);
        }

        [NotNull]
        public static Stream OpenWrite([NotNull] this string path, FileShare share, bool delete = true)
        {
            Argument.NotNull(path, nameof(path));
            if (delete)
                path.DeleteFile();

            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, share);
        }

        [NotNull]
        public static byte[] ReadAllBytesIfExis([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return !File.Exists(path) ? new byte[0] : File.ReadAllBytes(path);
        }

        [NotNull]
        public static byte[] ReadAllBytes([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return File.ReadAllBytes(path);
        }

        [NotNull]
        public static string ReadTextIfExis([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        [NotNull]
        public static string ReadTextIfExis([NotNull] this string workingDirectory, [NotNull] string subPath)
        {
            Argument.NotNull(workingDirectory, nameof(workingDirectory));
            Argument.NotNull(subPath, nameof(subPath));
            return ReadTextIfExis(CombinePath(workingDirectory, subPath));
        }

        [NotNull]
        public static IEnumerable<string> ReadTextLinesIfExis([NotNull] this string path)
        {
            Argument.NotNull(path, nameof(path));
            if (!File.Exists(path)) yield break;

            using (var reader = File.OpenText(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;

                    yield return line;
                }
            }
        }

        public static bool TryCreateUriWithoutScheme([NotNull] this string str, out Uri uri, [NotNull] params string[] scheme)
        {
            Argument.NotNull(str, nameof(str));
            Argument.NotNull(scheme, nameof(scheme));
            var flag = Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var target);

            // ReSharper disable once AccessToModifiedClosure
            if (flag)
            {
                foreach (var s in scheme.Where(s => flag))
                    flag = target.Scheme != s;
            }

            uri = flag ? target : null;

            return flag;
        }

        public static void WriteTextContentTo([NotNull] this string content, [NotNull] string path)
        {
            Argument.NotNull(content, nameof(content));
            Argument.NotNull(path, nameof(path));
            File.WriteAllText(path, content);
        }


        public static void WriteTextContentTo([NotNull] this string content, [NotNull] string workingDirectory, [NotNull] string path)
        {
            Argument.NotNull(content, nameof(content));
            Argument.NotNull(workingDirectory, nameof(workingDirectory));
            Argument.NotNull(path, nameof(path));
            WriteTextContentTo(content, CombinePath(workingDirectory, path));
        }
    }
}