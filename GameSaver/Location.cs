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
            public enum PathType
            {
                ANY,
                FILE,
                DIRECTORY
            }

            public Path(string source, string destination, PathType type)
            {
                Source = source;
                Destination = destination;
                Type = type;
            }

            public string Source { get; set; }
            public string Destination { get; set; }
            public PathType Type { get; set; }
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

        public bool HasChanged()
        {
            foreach(Path path in Paths)
            {
                switch(path.Type)
                {
                    case Path.PathType.DIRECTORY:
                        if (Directory.Exists(path.Source) && Directory.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                            return true;
                        break;
                    case Path.PathType.FILE:
                        if (File.Exists(path.Source) && File.GetLastWriteTimeUtc(path.Source) > DateTime_LastUpdated)
                            return true;
                        break;
                }
            }

            return false;
        }
    }
}
