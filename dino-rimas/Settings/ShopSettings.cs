using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text;
using DinoRimas.Models;
using System.Numerics;

namespace DinoRimas.Settings
{
    public class ShopSettings
    {
        private static ShopSettings _instance;
        private Dictionary<string, string> _names;
        private List<DinoShopModel> _shopList;
        private List<Vector3> _positions;
        private Random _rand;
        private List<string> _hasSub;
        private ShopSettings()
        {
            using var r = new StreamReader("DinoNames.json", Encoding.UTF8);
            _names = JsonSerializer.Deserialize<Dictionary<string, string>>(r.ReadToEnd());
            _shopList = GetDinoList();
            _rand = new Random(DateTime.Now.Second);
            _positions = new List<Vector3>
            {
                new Vector3(-462282.156f, -54966.359f, -73247.383f),
                new Vector3(-517779.062f ,128792.383f ,-70784.594f),
                new Vector3(-237888.953f ,363150.5f ,-69253.328f),
                new Vector3(484007.688f ,195747.594f ,-72550.602f),
                new Vector3(-21750.398f ,85635.734f ,-68614.961f),
                new Vector3(46536.098f ,-186975.156f ,-65410.535f),
                new Vector3(-169665.875f ,-585717.5f ,-72682.609f)
            };
            _hasSub = new List<string>
            {
                "RexAdultS",
                "TrikeAdultS",
                "GigaAdultS"
            };
        }
        private static ShopSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new ShopSettings();
                return _instance;
            }
        }

        public static bool hasSub(string adult)
        {
            return Instance._hasSub.Contains(adult);
        }
        public static string GetPositionById(int id)
        {
            var result = "";
            if(Instance._positions.Count > id)
            {            
                var pos = Instance._positions[id];
                result = $"X={pos.X} Y={pos.Y} Z={pos.Z}".Replace(',', '.');
            }
            return result;
        }
        public static string GetPositionRandom()
        {
            var result = "";
            var index = Instance._rand.Next(0, Instance._positions.Count - 1);           
            var pos = Instance._positions[index];
            result = $"X={pos.X} Y={pos.Y} Z={pos.Z}".Replace(',', '.');
            return result;
        }

        public static string GetName(string className)
        {
            if (className == null || Instance._names.ContainsKey(className)) return Instance._names[className];
            else return className;
        }
        public static List<DinoShopModel> GetShopList()
        {
            return Instance._shopList;
        }
        public static bool ClassExixsInShopList(string className)
        {
            return Instance._shopList.Any(d=>d.CharacterClass == className);
        }
        public static DinoShopModel GetShopDinoById(int id)
        {           
            return Instance._shopList.FirstOrDefault(d=>d.Id == id);
        }
        public static DinoShopModel GetShopDinoByClass(string className)
        {
            return Instance._shopList.FirstOrDefault(d => d.CharacterClass == className);
        }
        private List<DinoShopModel> GetDinoList()
        {
            using StreamReader r = new StreamReader("DinoShopConfig.json", Encoding.UTF8);
            var dinoList =  JsonSerializer.Deserialize<List<DinoShopModel>>(r.ReadToEnd());
            for (int i = 0; i < dinoList.Count; i++) dinoList[i].Id = i;
            return dinoList;
        }
    }
}
