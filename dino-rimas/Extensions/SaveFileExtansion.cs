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
            try
            {
                using var r = new StreamReader(path);
                return JsonSerializer.Deserialize<DinoModel>(r.ReadToEnd());
            }
            catch (Exception)
            {

                return null;
            }
        }
        public static DinoModel GetSaveFile(string path)
        {
            try
            {
                using var r = new StreamReader(path);
                return JsonSerializer.Deserialize<DinoModel>(r.ReadToEnd());
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static bool ExistsSaveFile(this SettingsModel settings, UserModel user, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            return File.Exists(path);
        }

        public static bool DeleteSaveFile(this SettingsModel settings, UserModel user, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            try
            {
                if( File.Exists(path)) File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public static bool DeleteSaveFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool UpdateSaveFile(this SettingsModel settings, UserModel user, DinoModel save, int server = -1)
        {
            var index = server < 0 ? user.Server : server;
            var path = settings.GameSaveFolderPath[index] + @"\" + user.Steamid + ".json";
            try
            {
                using var r = new StreamWriter(path);
                save.DNA = "Active";
                r.Write(JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
        public static bool UpdateSaveFile(string path, DinoModel save, int serverId)
        {
            try
            {
                using var r = new StreamWriter(path);
                save.DNA = "Active";
                r.Write(JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
