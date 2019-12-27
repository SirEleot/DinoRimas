using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class SettingsModel
    {
        public string GameSaveFolderPath { get; set; }
        public string SteamApiKey { get; set; }
        public string ConnectionString { get; set; }
        public UnitPayModel UnitPay { get; set; }
        public List<string[]> Contacts { get; set; }

    }
}
