using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameSaver
{
    class Location
    {
        private DateTime _lastUpdated;

        public class Path
        {
            public Path(string source, string destination, string regEx)
            {
                Source = source;
                Destination = destination;
                RegEx = regEx;
            }

            public string Source { get; set; }
            public string Destination { get; set; }
            public string RegEx { get; set; }
        }

        public class Process
        {
            public Process(string processName)
            {
                ProcessName = processName;
            }

            public string ProcessName { get; set; }
        }

        public string Name { get; set; }
        public ICollection<Path> Paths { get; set; }
        public Process AssociatedProcess { get; set; }
        public DateTime? LastUpdated { get => _lastUpdated; set => _lastUpdated = value ?? new DateTime(); }

        public Path[] GetChangedFiles()
        {
            List<Path> changedFiles = new List<Path>();

            foreach (Path path in Paths)
            {
                if (Directory.Exists(path.Source))
                {
                    path.Source = AddDirectorySeperator(path.Source);
                    path.Destination = AddDirectorySeperator(path.Destination);
                    changedFiles.AddRange(GetChangedFiles(path));
                }
                else if (File.Exists(path.Source) && File.GetLastWriteTimeUtc(path.Source) > LastUpdated)
                {
                    if(path.RegEx == null || Regex.IsMatch(System.IO.Path.GetFileName(path.Source), path.RegEx))
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
                changedFiles.AddRange(GetChangedFiles(new Path(AddDirectorySeperator(dir), path.Destination + GetRelativePath(path.Source, AddDirectorySeperator(dir)), path.RegEx)));
            }

            foreach (string file in Directory.GetFiles(path.Source))
            {
                if ((path.RegEx == null || Regex.IsMatch(System.IO.Path.GetFileName(file), path.RegEx)) && File.GetLastWriteTimeUtc(file) > LastUpdated)
                    changedFiles.Add(new Path(file, path.Destination + GetRelativePath(path.Source, file), path.RegEx));
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
        protected static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

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

        protected static string AddDirectorySeperator(string input)
        {
            if (!input.ElementAt(input.Length - 1).Equals(System.IO.Path.DirectorySeparatorChar) &&
                       !input.ElementAt(input.Length - 1).Equals(System.IO.Path.AltDirectorySeparatorChar))
                return input + System.IO.Path.DirectorySeparatorChar;
            return input;
        }
    }
}
