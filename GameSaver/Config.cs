using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSaver
{
    class Config
    {
        public ICollection<Location> Locations;

        public Config()
        {
            Locations = new List<Location>();
        }

        public Config(ICollection<Location> locations)
        {
            Locations = locations ?? throw new ArgumentNullException(nameof(locations));
        }

        public static Config Load(string file)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
