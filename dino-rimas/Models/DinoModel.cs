using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DinoRimas.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DinoRimas.Models
{
    public class DinoModel
    {
        public int Id { get; set; }
        public string Name { 
            get {
                return ShopSettings.GetName(CharacterClass);
            }
        }
        public int Server { get; set; }
        public string CharacterClass { get; set; }
        public string DNA { get; set; } = "";
        public string Location_Isle_V3 { get; set; }
        public string Rotation_Isle_V3 { get; set; } = "P=0.0 Y=0.0 R=0.0";
        public string Growth { get; set; } = "1.0";
        public string Hunger { get; set; } = "99999";
        public string Thirst { get; set; } = "99999";
        public string Stamina { get; set; } = "99999";
        public string Health { get; set; } = "99999";
        public string BleedingRate { get; set; } = "0";
        public string Oxygen { get; set; } = "40";
        public bool bGender { get; set; } = false;
        public bool bIsResting { get; set; } = false;
        public bool bBrokenLegs { get; set; } = false;
        public string ProgressionPoints { get; set; } = "0";
        public string ProgressionTier { get; set; } = "1";
        public string UnlockedCharacters { get; set; } = "";
        public string CameraRotation_Isle_V3 { get; set; } = "P=0.0 Y=0.0 R=0.0";
        public string CameraDistance_Isle_V3 { get; set; } = "0";
        public int SkinPaletteSection1 { get; set; } = 0;
        public int SkinPaletteSection2 { get; set; } = 0;
        public int SkinPaletteSection3 { get; set; } = 0;
        public int SkinPaletteSection4 { get; set; } = 0;
        public int SkinPaletteSection5 { get; set; } = 0;
        public int SkinPaletteSection6 { get; set; } = 0;
        public string SkinPaletteVariation { get; set; } = "6.0";
        [JsonIgnore]
        public DateTime CraetionAs { get; set; } = DateTime.Now;
        public bool Active { get; set; } = false;

        public void UpdateFrom(DinoModel model)
        {
            CharacterClass = model.CharacterClass;
            Location_Isle_V3 = model.Location_Isle_V3;
            Rotation_Isle_V3 = model.Rotation_Isle_V3;
            Health = model.Health;
            Hunger = model.Hunger;
            Growth = model.Growth;
            Thirst = model.Thirst;
            Oxygen = model.Oxygen;
            BleedingRate = model.BleedingRate;
            bIsResting = model.bIsResting;
            bBrokenLegs = model.bBrokenLegs;
        }

        public void ResponsePrepair()
        {
            Hunger = null;
            Thirst = null;
            Stamina = null;
            Health = null;
            DNA = null;
            Location_Isle_V3 = null;
            Rotation_Isle_V3 = null;
            //Growth = null;
            BleedingRate = null;
            Oxygen = null;
            ProgressionTier = null;
            ProgressionPoints = null;
            UnlockedCharacters = null;
            CameraDistance_Isle_V3 = null;
            CameraRotation_Isle_V3 = null;
            SkinPaletteVariation = null;
        }
       
    }
}
