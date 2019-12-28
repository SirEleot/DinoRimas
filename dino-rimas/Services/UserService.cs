using DinoRimas.Data;
using DinoRimas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using DinoRimas.Extensions;

namespace DinoRimas.Services
{
    struct SteamInfo
    {
        public string steamid { get; set; }
        public string personaname { get; set; }
        public string avatarfull { get; set; }
        public string gameserverip { get; set; }
    }
    public class UserService
    {
        DinoRimasDbContext _context;
        SettingsModel _settings;
        ClaimsPrincipal User { get; }
        public UserService(IHttpContextAccessor accessor, DinoRimasDbContext context, IOptions<SettingsModel> settings)
        {
            _context = context;
            _settings = settings.Value;
            User = accessor.HttpContext.User;
        }

        private UserModel GetDinoUser()
        {
            var steamid = User.Identity.IsAuthenticated ? (User.Claims.First().Value.Split('/').Last()) : null;
            if (steamid == null) return null;

            var dinoUser = _context.User.FirstOrDefault(u => u.Steamid == steamid);
            if (dinoUser == null)
            {
                var info = GetSteamInfo(steamid);
                dinoUser = new UserModel
                {
                    Steamid = steamid,
                    ProfileName = info.personaname,
                    ProfileImg = info.avatarfull,
                    Balance = 100000,
                    Slots = 2,
                    Inventory = new List<DinoModel>()
                };
                var file = _settings.GetSaveFile(dinoUser);
                if(file != null)
                {
                    var dino = new DinoModel
                    {
                        IsAlive = true,
                        Vip = false,
                        CraetionAs = DateTime.Now,
                        Config = file,
                        IsActivated = true,
                        Name = file.CharacterClass
                    };
                    _context.User.Add(dinoUser);
                    dinoUser.Inventory.Add(dino);
                }
                _context.User.Add(dinoUser);
                _context.SaveChanges();
            }
            else
            {                
                dinoUser.Inventory = _context.Entry(dinoUser)
                    .Collection(d => d.Inventory)             
                    .Query()
                    .Where(d=>d.IsAlive)
                    .ToList();
            }
            return dinoUser;
        }

        private SteamInfo GetSteamInfo(string steamid)
        {
            var str = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v002/?key={_settings.SteamApiKey}&steamids={steamid}&format=json";
            var o = JObject.Parse(new WebClient().DownloadString(str));
            var p = o.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Name == "players")
                .Select(p => p.Value);
            return p.First().ToObject<SteamInfo[]>().FirstOrDefault();
        }

        public async Task<UserModel> GetDinoUserAsync()
        {
            return await Task.Run(() => GetDinoUser());
        }

        public DinoModel CreateNewDino(UserModel user, DinoShopModel dinoShop)
        {

            var dino = new DinoModel
            {
                Name = dinoShop.Name,
                IsActivated = false,
                IsAlive = true,
                Vip = true,
                Config = dinoShop.BaseConfig,
                CraetionAs = DateTime.Now
            };
            if (!user.Inventory.Any(d => d.IsActivated))
            {
                dino.IsActivated = true;
                _settings.AddSaveFile(user, dino.Config);
            }
            return dino;
        }

        public DinoModel CreateNewDino(UserModel user, DinoSaveModel config, bool active = true)
        {

            var dino = new DinoModel
            {
                Name = config.CharacterClass,
                IsActivated = active,
                IsAlive = true,
                Vip = true,
                Config = config,
                CraetionAs = DateTime.Now
            };
            return dino;
        }

    }
}
