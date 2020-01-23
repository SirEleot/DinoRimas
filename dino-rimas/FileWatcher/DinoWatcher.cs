using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DinoRimas.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DinoRimas.FileWatcher
{
    public class DinoWatcher
    {
        int _id;
        private static List<DinoWatcher> _watchers;
        private static SettingsModel _settings;
        FileSystemWatcher _w;
        private DinoWatcher(int id)
        {
            _id = id;
            _w = new FileSystemWatcher(_settings.GameSaveFolderPath[_id]);
            _w.Filter = "*.json";
            _w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _w.IncludeSubdirectories = false;
            _w.Changed += OnChange;
            _w.Created += OnCreate;
            _w.Deleted += OnDelete;
            _w.EnableRaisingEvents = true;
        }

        public static void Run(SettingsModel settings)
        {
            _settings = settings;
            if (_watchers == null) _watchers = new List<DinoWatcher>();
            for (int i = 0; i < _settings.GameSaveFolderPath.Count; i++)
            {
                _watchers.Add(new DinoWatcher(i));
            }
        }

        private void OnDelete(object sender, FileSystemEventArgs e)
        {
            try
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];

            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
            finally
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = true;
                var steamid = e.Name[0..^5];

            }
        }

        private void OnCreate(object sender, FileSystemEventArgs e)
        {
            try
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];

            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
            finally
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = true;
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = false;

            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
            finally
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = true;
            }
        }

        private void GetUser()
        {

        }
    }
}
