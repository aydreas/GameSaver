using System;
using System.Collections.Generic;
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
            Console.WriteLine(c.ToString());
            Console.ReadLine();
        }
    }
}
