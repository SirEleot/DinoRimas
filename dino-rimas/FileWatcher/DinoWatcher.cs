using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DinoRimas.Models;
using DinoRimas.Data;
using Microsoft.EntityFrameworkCore;
using static DinoRimas.Extensions.SaveFileExtansion;

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
            var dir = _settings.GameSaveFolderPath[_serverId];
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            _w = new FileSystemWatcher(dir);
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
            var option = new DbContextOptionsBuilder();
            option.UseNpgsql(connString);
            _option = option.Options;
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
                _w.EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u => u.Inventory)
                    .FirstOrDefault(u => u.Steamid == steamid);
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
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
            }
            finally
            {
                _w.EnableRaisingEvents = true;
            }
        }

        internal static void Start(int serverId)
        {
            _watchers[serverId]._w.EnableRaisingEvents = true;
        }

        internal static void Stop(int serverId)
        {
            _watchers[serverId]._w.EnableRaisingEvents = false;
        }

        private void OnCreate(object sender, FileSystemEventArgs e)
        {
            try
            {
                _w.EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u=>u.Inventory)
                    .FirstOrDefault(u => u.Steamid == steamid);
                
                if(user != null)
                {
                    var saveFile = GetSaveFile(e.FullPath);
                    if (saveFile.Id == default)
                    {
                        saveFile.Active = true;
                        saveFile.Server = _serverId;
                        user.Inventory.Add(saveFile);
                        context.SaveChanges();
                        UpdateSaveFile(e.FullPath, saveFile, _serverId);
                    }
                    else
                    {
                        var dino = user.Inventory.FirstOrDefault(d => d.Id == saveFile.Id);
                        if(dino != null)
                        {
                            dino.Active = true;
                            context.SaveChanges();
                        }
                        AddUserBan(60, user, "Не вышел с сервера при деактивации");
                    }
                }
            }
            catch (Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
            }
            finally
            {
                _w.EnableRaisingEvents = true;
            }
        }

        private void AddUserBan(int minutes, UserModel user, string reason)
        {
            using var context = new DinoRimasDbContext(_option);
            var to = DateTime.Now.AddMinutes(minutes);
            user.BannedTo = to;
            context.Users.Update(user);
            var banLog = new BanLogModel
            {
                Reason = reason,
                Steamid = user.Steamid,
                DateFrom = DateTime.Now,
                DateTo = to
            };
            context.BanLogs.Add(banLog);
            context.SaveChanges();
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                _w.EnableRaisingEvents = false;
                var steamid = e.Name[0..^5];
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u => u.Inventory)
                    .FirstOrDefault(u => u.Steamid == steamid);
                if(user != null)
                {
                    var saveFile = GetSaveFile(e.FullPath);
                    if (saveFile.Id == default) return;
                    var dino = user.Inventory.FirstOrDefault(d => d.Id == saveFile.Id);
                    if (!dino.Active) {
                        var activeDino = user.Inventory.FirstOrDefault(d => d.Id == saveFile.Id && d.Active);
                        if (activeDino != null) activeDino.Active = false;
                    }
                    dino.Active = true;
                    dino.UpdateFrom(saveFile);
                    context.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
            }
            finally
            {
                _w.EnableRaisingEvents = true;
            }
        }
    }
}
