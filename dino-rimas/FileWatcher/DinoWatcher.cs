using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DinoRimas.Models;
using Microsoft.Extensions.DependencyInjection;
using DinoRimas.Data;
using Microsoft.EntityFrameworkCore;

namespace DinoRimas.FileWatcher
{
    public class DinoWatcher
    {
        int _serverId;
        private static List<DinoWatcher> _watchers;
        private static SettingsModel _settings;
        FileSystemWatcher _w;
        static DbContextOptions _option;
        private DinoWatcher(int id)
        {
            _serverId = id;
            _w = new FileSystemWatcher(_settings.GameSaveFolderPath[_serverId]);
            _w.Filter = "*.json";
            _w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _w.IncludeSubdirectories = false;
            _w.Changed += OnChange;
            _w.Created += OnCreate;
            _w.Deleted += OnDelete;
            _w.EnableRaisingEvents = true;
        }

        public static void Run(SettingsModel settings, string connString)
        {
            _settings = settings;
            _option = new DbContextOptionsBuilder().UseNpgsql(connString).Options;
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
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users.FirstOrDefault(u => u.Steamid == steamid);
                if(user != null)
                {
                    var currentDino = user.Inventory.FirstOrDefault(d => d.Active && d.Server == _serverId);
                    if(currentDino != null)
                    {
                        user.Inventory.Remove(currentDino);
                        currentDino.DNA = "Умер";
                        context.SaveChanges();
                    }
                    else 
                    {
                        user.ChangeOnServer++;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc.Message));
            }
            finally
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = true;
            }
        }

        private void OnCreate(object sender, FileSystemEventArgs e)
        {
            try
            {
                (sender as FileSystemWatcher).EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users.FirstOrDefault(u => u.Steamid == steamid);
                if(user != null)
                {
                    var currentDino = user.Inventory.FirstOrDefault(d => d.Active && d.Server == _serverId);
                }
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
    }
}
