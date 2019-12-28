using DinoRimas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Extensions
{
    public static class SaveFileExtansion
    {
        public static DinoSaveModel GetSaveFile(this SettingsModel settings, UserModel user)
        {
            var path = settings.GameSaveFolderPath + @"\" + user.Steamid + ".json";
            if (File.Exists(path))
            {
                using var r = new StreamReader(path);
                return JsonConvert.DeserializeObject<DinoSaveModel>(r.ReadToEnd());
            }
            return null;
        }

        public static bool ExistsSaveFile(this SettingsModel settings, UserModel user)
        {
            var path = settings.GameSaveFolderPath + @"\" + user.Steamid + ".json";
            return File.Exists(path);
        }

        public static void AddSaveFile(this SettingsModel settings, UserModel user, DinoSaveModel save)
        {
            var path = settings.GameSaveFolderPath + @"\" + user.Steamid + ".json";
            using var r = new StreamWriter(path);
            r.Write(JsonConvert.SerializeObject(save));
        }
    }
}
