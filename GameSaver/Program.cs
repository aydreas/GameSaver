using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            Config c = Config.Load("config.json");
            Location loc = new Location();
            loc.LastUpdated = 10000;
            loc.Paths = new List<Location.Path>
            {
                new Location.Path("C:\\Users\\Andreas\\Desktop", "C:\\Users")
            };
            Console.WriteLine(String.Join("\n", loc.GetChangedFiles().Select(x => x.Source + ", " + x.Destination)));
            Console.ReadLine();
        }

        static void CopyFile(string source, string dest)
        {

        }
    }
}
