using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class PriceModel
    {
        public int Position { get; set; }
        public int Slot { get; set; }
        public int Sex { get; set; }
    }
    public class SettingsModel
    {
        public List<string> GameSaveFolderPath { get; set; }
        public string SteamApiKey { get; set; }
        public UnitPayModel UnitPay { get; set; }
        public List<string[]> Contacts { get; set; }
        public List<string> ServerIps { get; set; }
        public PriceModel Price { get; set; }

    }
}
