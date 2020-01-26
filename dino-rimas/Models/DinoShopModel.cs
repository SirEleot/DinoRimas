using DinoRimas.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class DinoShopModel
    {
        public int Id { get; set; }
        public string CharacterClass { get; set; }
        public int Price { get; set; }
        public int Sale { get; set; }
        public string Description { get; set; }
        public bool Survival { get; set; }
        public string Location_Isle_V3 { 
            get 
            {
                return ShopSettings.GetPositionRandom();
            }
        }
        public string Name
        {
            get
            {
                return ShopSettings.GetName(CharacterClass);
            }
        }
    }   
}
