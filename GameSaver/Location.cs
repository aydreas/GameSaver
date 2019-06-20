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

        public string[] GetChangedFiles()
        {
            List<string> changedFiles = new List<string>();

            foreach (Path path in Paths)
            {
                if (Directory.Exists(path.Source) && Directory.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                {
                    changedFiles.AddRange(GetChangedFiles(path.Source));
                }
                else if (File.Exists(path.Source) && File.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                {
                    changedFiles.Add(path.Source);
                }
            }

            return changedFiles.ToArray();
        }

        protected string[] GetChangedFiles(string path)
        {
            List<string> changedFiles = new List<string>();

            foreach (string dir in Directory.GetDirectories(path))
            {
                changedFiles.AddRange(GetChangedFiles(dir));
            }

            foreach (string file in Directory.GetFiles(path))
            {
                changedFiles.Add(file);
            }

            return changedFiles.ToArray();
        }

        public bool GetProcessState()
        {

            if (System.Diagnostics.Process.GetProcessesByName(AssociatedProcess.ProcessName).Length > 0)
                return true;
            return false;
        }
    }
}
