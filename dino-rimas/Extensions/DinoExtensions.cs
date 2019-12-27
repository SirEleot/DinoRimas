using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using DinoRimas.Models;
using DinoRimas.Data;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace DinoRimas.Extensions
{
    struct SteamInfo
    {
        public string steamid { get; set; }
        public string personaname { get; set; }
        public string avatarfull { get; set; }
    }
    public static class DinoExtensions
    {
        private static DinoRimasDbContext _context;
        private static DinoRimasDbContext context
        {
            get
            {
                if (_context == null) _context = new DinoRimasDbContext();
                return _context;
            }
        }
        private static UserModel GetDinoUser(ClaimsPrincipal user)
        {
            var steamid = user.Identity.IsAuthenticated ? (user.Claims.First().Value.Split('/').Last()) : null;
            if (steamid == null) return null;
            var dinoUser = context.User.FirstOrDefault(u => u.Steamid == steamid);
            if (dinoUser == null)
            {
                var info = GetSteamInfo(steamid);
                dinoUser = new UserModel
                {
                    Steamid = steamid,
                    ProfileName = info.personaname,
                    ProfileImg = info.avatarfull,
                    Balance = 0,
                    Slots = 2,
                    Inventory = new List<DinoModel>(),
                    BaseDinos = new List<DinoModel>()
                };
                context.User.Add(dinoUser);
                context.SaveChanges();
            }
            return dinoUser;
        }

        private static SteamInfo GetSteamInfo(string steamid)
        {
            var str = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v002/?key={Program.Settings.SteamApiKey}&steamids={steamid}&format=json";            
            var o = JObject.Parse(new WebClient().DownloadString(str));
            var p = o.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Name == "players")
                .Select(p => p.Value);
            return  p.First().ToObject<SteamInfo[]>().FirstOrDefault();
        }

        public static async Task<UserModel> GetDinoUserAsync(this ClaimsPrincipal user)
        {
             return await Task.Run(()=>GetDinoUser(user));
        }
    }
}
