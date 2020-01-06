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

        public async Task<UserModel> GetDinoUserAsync()
        {
            return await Task.Run(() => GetDinoUser());
        }

        public DinoModel CreateNewDino(DinoShopModel dinoShop, bool active)
        {
            var dino = new DinoModel
            {
                Image = dinoShop.Img,
                Name = dinoShop.Name,                
                IsActivated = active,
                IsAlive = true,
                Vip = true,
                Config = dinoShop.BaseConfig,
                CraetionAs = DateTime.Now
            };
            return dino;
        }

        public DinoModel CreateNewDino(DinoSaveModel config, bool active)
        {
            var shop = _context.ShopDinoList.FirstOrDefault(s => s.ClassName == config.CharacterClass);
            var name = config.CharacterClass;
            var img = "DefaultDino.png";

            if (shop != null)
            {
                name = shop.Name;
                img = shop.Img;
            }
            var dino = new DinoModel
            {
                Name = name,
                IsActivated = active,
                IsAlive = true,
                Vip = true,
                Config = config,
                Image = img,
                CraetionAs = DateTime.Now
            };
            return dino;
        }

        public bool UserOnServer(string steamid)
        {
            var info = GetSteamInfo(steamid);
            return (info.gameserverip != null &&  _settings.ServerIps.Any(ip=>ip == info.gameserverip));
        }

        private UserModel GetDinoUser()
        {
            var steamid = User.Identity.IsAuthenticated ? (User.Claims.First().Value.Split('/').Last()) : null;
            if (steamid == null) return null;

            var dinoUser = _context.User.FirstOrDefault(u => u.Steamid == steamid);
            if (dinoUser == null) dinoUser = CreateNewUser(steamid);
            else
            {
                dinoUser.Inventory = _context.Entry(dinoUser)
                    .Collection(d => d.Inventory)
                    .Query()
                    .Where(d => d.IsAlive)
                    .ToList();
            }
            return dinoUser;
        }

        private UserModel CreateNewUser(string steamid)
        {
            var info = GetSteamInfo(steamid);
            var dinoUser = new UserModel
            {
                Steamid = steamid,
                ProfileName = info.personaname,
                ProfileImg = info.avatarfull,
                Balance = 100000,
                Slots = 2,
                Inventory = new List<DinoModel>()
            };
            var save = _settings.GetSaveFile(dinoUser);
            if (save != null)
            {
                var dino = CreateNewDino(save, true);
                dinoUser.Inventory.Add(dino);
            }
            _context.User.Add(dinoUser);
            _context.SaveChanges();
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

    }
}
