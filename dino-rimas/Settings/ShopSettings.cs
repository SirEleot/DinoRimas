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
                new Vector3(-169665.875f ,-585717.5f ,-72682.609f),

                new Vector3(-588464.75f,  -229543.391f, -32074.162f),
                new Vector3(-395896.844f, -269987.312f, -66561.102f),
                new Vector3(-191814.516f, -373199.594f, -40851.805f),
                new Vector3(13567.566f, -404817.188f, -42980.176f),
                new Vector3(107723.289f, 283723.094f, -40183.656f),
                new Vector3(-205974.016f, -30537.402f, -64633.398f),
                new Vector3(-410652.125f, 471226.312f, -29178.383f),
                new Vector3(-324573.219f, 151895.031f, -65705.188f),
                new Vector3(303773.5f, -134582.484f, -24480.203f)
            };
            //_positions = new List<Vector3>
            //{
            //    new Vector3(141841.641f, 43571.215f, -63106.398f),
            //    new Vector3(2019.969f, 233578.688f, -43503.676f),
            //    new Vector3(186709.969f, 199657.719f, -56242.723f),
            //    new Vector3(-156474.188f, 245331.562f, -22110.965f),
            //    new Vector3(-245081.875f, 138146.094f, -28058.467f),
            //    new Vector3(-127679.211f, -71919.625f, -28809.916f),
            //    new Vector3(-86821.18f, -232605.969f, -27385.969f),
            //    new Vector3(40336.242f, -149336.406f, -47221.109f),
            //    new Vector3(222678.75f, -139325.172f, -43596.402f),
            //    new Vector3(-12402.737f, 36912.574f, -59624.219f)
            //};
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
