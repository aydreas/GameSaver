using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSaver
{
    class Location
    {
        public class Path
        {
            public Path(string source, string destination)
            {
                Source = source;
                Destination = destination;
            }

            public string Source { get; set; }
            public string Destination { get; set; }
        }

        public class Process
        {
            public Process(string processName)
            {
                ProcessName = processName;
            }

            public string ProcessName { get; set; }
        }

        private long _LastUpdated;
        private DateTime _DateTime_LastUpdated;

        protected DateTime DateTime_LastUpdated
        {
            get => _DateTime_LastUpdated;
            set
            {
                _DateTime_LastUpdated = value;
                _LastUpdated = ToUnix(value);
            }
        }

        public string Name { get; set; }
        public ICollection<Path> Paths { get; set; }
        public Process AssociatedProcess { get; set; }
        public long LastUpdated
        {
            get => _LastUpdated;
            set
            {
                _LastUpdated = value;
                _DateTime_LastUpdated = FromUnix(value);
            }
        }

        protected static long ToUnix(DateTime dateTime) => (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + 1;
        protected static DateTime FromUnix(long unixTime) => new DateTime(1970, 1, 1).AddSeconds(unixTime);

        public bool CheckValidity()
        {
            foreach (Path path in Paths)
            {
                if (Directory.Exists(path.Source) && Directory.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                    return false;
                else if (File.Exists(path.Source) && File.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                    return false;
            }

            return true;
        }

        public Path[] GetChangedFiles()
        {
            List<Path> changedFiles = new List<Path>();

            foreach (Path path in Paths)
            {
                if (Directory.Exists(path.Source) && Directory.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                {
                    if (!path.Destination.ElementAt(path.Destination.Length - 1).Equals(System.IO.Path.DirectorySeparatorChar) &&
                       !path.Destination.ElementAt(path.Destination.Length - 1).Equals(System.IO.Path.AltDirectorySeparatorChar))
                        path.Destination += System.IO.Path.DirectorySeparatorChar;

                    changedFiles.AddRange(GetChangedFiles(path));
                }
                else if (File.Exists(path.Source) && File.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                {
                    changedFiles.Add(path);
                }
            }

            return changedFiles.ToArray();
        }

        protected Path[] GetChangedFiles(Path path)
        {
            List<Path> changedFiles = new List<Path>();

            foreach (string dir in Directory.GetDirectories(path.Source))
            {
                changedFiles.AddRange(GetChangedFiles(new Path(dir, path.Destination + GetRelativePath(path.Source, dir))));
            }

            foreach (string file in Directory.GetFiles(path.Source))
            {
                changedFiles.Add(new Path(file, path.Destination + GetRelativePath(path.Source, file)));
            }

            return changedFiles.ToArray();
        }

        public bool GetProcessState()
        {
            if (AssociatedProcess == null)
                return false;

            if (System.Diagnostics.Process.GetProcessesByName(AssociatedProcess.ProcessName).Length > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        protected static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!System.IO.Path.HasExtension(path) &&
                !path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                return path + System.IO.Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}
