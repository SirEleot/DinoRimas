using DinoRimas.Data;
using DinoRimas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public DinoModel CreateNewDino(DinoShopModel dinoShop, int server)
        {
            var dino = new DinoModel
            {
                CharacterClass = dinoShop.CharacterClass,
                Server = server,
                Location_Isle_V3 = dinoShop.Location_Isle_V3,
                CraetionAs = DateTime.Now
            };
            return dino;
        }

        private UserModel GetDinoUser()
        {
            var steamid = User.Identity.IsAuthenticated ? (User.Claims.First().Value.Split('/').Last()) : null;
            if (steamid == null) return null;

            var dinoUser = _context.Users.FirstOrDefault(u => u.Steamid == steamid);
            if (dinoUser == null) dinoUser = CreateNewUser(steamid);
            else
            {
                dinoUser.Inventory = _context.Entry(dinoUser)
                    .Collection(d => d.Inventory)
                    .Query()
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
                Balance = 0,
                Server = 0,
                Inventory = new List<DinoModel>()
            };
            if (dinoUser.IsAdmin) dinoUser.Balance = 100000;
            dinoUser.Slots = new List<int>();
            for (int i = 0; i < _settings.GameSaveFolderPath.Count; i++)
            {
                dinoUser.Slots.Add(2);
                var currentDino = _settings.GetSaveFile(dinoUser, i);
                if (currentDino != null)
                {
                    currentDino.Server = i;
                    currentDino.Active = true;
                    if (currentDino.Id > 0) currentDino.Id = default;
                    dinoUser.Inventory.Add(currentDino); 
                }
            }
            _context.Users.Add(dinoUser);
            _context.SaveChanges();
            foreach (var dino in dinoUser.Inventory)
            {
                _settings.UpdateSaveFile(dinoUser, dino, dino.Server);
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

    }
}
