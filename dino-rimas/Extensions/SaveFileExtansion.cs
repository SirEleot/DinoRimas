using DinoRimas.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinoRimas.FileWatcher;

namespace DinoRimas.Extensions
{
    public static class SaveFileExtansion
    {
        public static DinoModel GetSaveFile(this SettingsModel settings, UserModel user, int server = -1 )
        {
            var index = server < 0 ? user.Server : server;
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            if (File.Exists(path))
            {
                using var r = new StreamReader(path);
                return JsonSerializer.Deserialize<DinoModel>(r.ReadToEnd()); 
            }
            return null;
        }
        public static DinoModel GetSaveFile(string path)
        {           
            if (File.Exists(path))
            {
                using var r = new StreamReader(path);
                return JsonSerializer.Deserialize<DinoModel>(r.ReadToEnd());
            }
            return null;
        }

        public static bool ExistsSaveFile(this SettingsModel settings, UserModel user, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            return File.Exists(path);
        }

        public static void DeleteSaveFile(this SettingsModel settings, UserModel user, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            DinoWatcher.Stop(index);
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            if( File.Exists(path)) File.Delete(path);
            DinoWatcher.Start(index);
        }

        public static void UpdateSaveFile(this SettingsModel settings, UserModel user, DinoModel save, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            DinoWatcher.Stop(index);
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            using var r = new StreamWriter(path);
            r.Write(JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true}));
            DinoWatcher.Start(index);
        }
        public static void UpdateSaveFile(string path, DinoModel save, int serverId)
        {
            DinoWatcher.Stop(serverId);
            using var r = new StreamWriter(path);
            r.Write(JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true }));
            DinoWatcher.Start(serverId);
        }
    }
}
