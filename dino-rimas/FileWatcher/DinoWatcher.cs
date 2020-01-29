﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DinoRimas.Models;
using DinoRimas.Data;
using Microsoft.EntityFrameworkCore;
using static DinoRimas.Extensions.SaveFileExtansion;
using System.Threading;

namespace DinoRimas.FileWatcher
{
    public class DinoWatcher
    {
        int _serverId;
        private static List<DinoWatcher> _watchers;
        private static SettingsModel _settings;
        private readonly FileSystemWatcher _w;
        static DbContextOptions _option;
        public static Queue<DinoWatcherData> DinoWatcherQueue;
        private static Thread _thread;
        private static Dictionary<string, DateTime> _lastUserUpdate;
        private DinoWatcher(int id)
        {
            _serverId = id;
            var dir = _settings.GameSaveFolderPath[_serverId];
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            _w = new FileSystemWatcher(dir)
            {
                Filter = "*.json",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                IncludeSubdirectories = false
            };
            _w.Created += (object sender, FileSystemEventArgs e) => DinoWatcherQueue.Enqueue(new DinoWatcherData(e, _serverId));
            _w.Deleted += (object sender, FileSystemEventArgs e) => DinoWatcherQueue.Enqueue(new DinoWatcherData(e, _serverId));
            _w.Changed += (object sender, FileSystemEventArgs e) => AddToQueue(e);
            _w.EnableRaisingEvents = true;
        }

        private void AddToQueue(FileSystemEventArgs e)
        {
            if (!_lastUserUpdate.ContainsKey(e.Name)) _lastUserUpdate.Add(e.Name, DateTime.Now);
            if (_lastUserUpdate[e.Name] > DateTime.Now) return;
            DinoWatcherQueue.Enqueue(new DinoWatcherData(e, _serverId));
            _lastUserUpdate[e.Name] = DateTime.Now.AddMilliseconds(200);
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
            DinoWatcherQueue = new Queue<DinoWatcherData>();
            _lastUserUpdate = new Dictionary<string, DateTime>();
            _thread = new Thread(DinoQueueWatch);
            _thread.Start();
        }


        private static void DinoQueueWatch()
        {   
            while (true)
            {
                if (DinoWatcherQueue.Count > 0)
                {
                    var data = DinoWatcherQueue.Dequeue();
                    switch (data.Type) 
                    {
                        case WatcherChangeTypes.Created:
                            if (!OnCreate(data)) DinoWatcherQueue.Enqueue(data);
                            break;
                        case WatcherChangeTypes.Deleted:
                            if (!OnDelete(data)) DinoWatcherQueue.Enqueue(data);
                            break;
                        case WatcherChangeTypes.Changed:
                            if (!OnChange(data)) DinoWatcherQueue.Enqueue(data);
                            break;                           
                        default: continue;
                    }
                }
                else Thread.Sleep(50);
            }
        }
        private static bool OnCreate(DinoWatcherData data)
        {
            try
            {
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u => u.Inventory)
                    .FirstOrDefault(u => u.Steamid == data.Steamid);
                if (user == null) return true;

                var saveFile = GetSaveFile(data.Path);
                if (user.DeactivaionTime < DateTime.Now)
                {
                    if(saveFile.Id == default)
                    {
                        saveFile.Server = data.ServerId;
                        saveFile.Active = true;
                        user.Inventory.RemoveAll(d => d.Active && d.Server == data.ServerId);                    
                        user.Inventory.Add(saveFile);
                        context.SaveChanges();

                        var counter = 0;
                        while (counter < 3 || !UpdateSaveFile(data.Path, saveFile, data.ServerId))
                        {
                            counter++;
                            Thread.Sleep(100);
                        };
                    }
                   
                    return true;
                }
                else
                {
                     user.Inventory.RemoveAll(d => d.Deactivated);
                     context.SaveChanges();
                     AddUserBan( user, "Не вышел с сервера при деактивации");
                     var counter = 0;
                     while (counter < 3 || !DeleteSaveFile(data.Path))
                     {
                         counter++;
                         Thread.Sleep(100);
                     };
                    return true;
                }
            }
            catch( Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
                return false;
            }
            
        }

        private static bool OnDelete(DinoWatcherData data)
        {
            try
            {
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u => u.Inventory)
                    .FirstOrDefault(u => u.Steamid == data.Steamid);
                if (user == null) return true;
                var currentDino = user.Inventory.FirstOrDefault(d => d.Active && d.Server == data.ServerId);
                if (currentDino != null)
                {
                    user.Inventory.Remove(currentDino);
                    currentDino.DNA = $"{user.ProfileName}({user.Steamid}) Умер";                    
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
                return false;
            }
           
        }

        private static bool OnChange(DinoWatcherData data)
        {
            try
            {
                using var context = new DinoRimasDbContext(_option);
                var user = context.Users
                    .Include(u => u.Inventory)
                    .FirstOrDefault(u => u.Steamid == data.Steamid);
                if (user == null) return true;
                if(user.Banned) return true;
                var saveFile = GetSaveFile(data.Path);
                if (saveFile == null) return true;
                if (saveFile.Id == default) return true;

                var dino = user.Inventory.FirstOrDefault(d => d.Id == saveFile.Id);
                if (dino == null) return true;
                if (!dino.Active)
                {
                    var activeDino = user.Inventory.FirstOrDefault(d => d.Server == data.ServerId && d.Active);
                    if (activeDino != null) activeDino.Active = false; 
                    dino.Active = true;
                }
                dino.UpdateFromGame(saveFile);
                context.SaveChanges();
                return true;
            }
            catch (Exception exc)
            {
                using var context = new DinoRimasDbContext(_option);
                context.Errors.Add(new ErrorLogsModel(exc));
                context.SaveChanges();
                return false;
            }
        }

        private static void AddUserBan(UserModel user, string reason)
        {
            using var context = new DinoRimasDbContext(_option);
            var to = DateTime.Now.AddMinutes(60 * user.ChangeOnServer);
            user.BannedTo = to;
            user.ChangeOnServer++;
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
    }
}
