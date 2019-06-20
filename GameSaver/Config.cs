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
        public ICollection<Location> Locations { get; set; }
        public int RefreshInterval { get; set; }

        public Config()
        {
            Locations = new List<Location>();
        }

        public Config(ICollection<Location> locations)
        {
            Locations = locations ?? throw new ArgumentNullException(nameof(locations));
        }

        public Config(ICollection<Location> locations, int refreshInterval) : this(locations)
        {
            RefreshInterval = refreshInterval;
        }

        public static Config Load(string file)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
        }

        public void Save(string file)
        {
            File.WriteAllText(file, ToString());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
