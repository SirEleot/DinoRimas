using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool Active { get; set; } = false;
        public bool Deactivated { get; set; } = false;
        [JsonIgnore]
        public int Server { get; set; }
        [JsonIgnore]
        public DateTime CraetionAs { get; set; } = DateTime.Now;

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

        public void UpdateFromGame(DinoModel model)
        {
            CharacterClass = model.CharacterClass;
            Location_Isle_V3 = model.Location_Isle_V3;
            Rotation_Isle_V3 = model.Rotation_Isle_V3;
            Growth = model.Growth;
            Hunger = model.Hunger;
            Thirst = model.Thirst;
            Stamina = model.Stamina;
            Health = model.Health;
            BleedingRate = model.BleedingRate;
            Oxygen = model.Oxygen;
            bGender = model.bGender;
            bIsResting = model.bIsResting;
            bBrokenLegs = model.bBrokenLegs;
        }
        public void UpdateFromGameFull(DinoModel model)
        {
            CharacterClass = model.CharacterClass;
            DNA = model.DNA;
            Location_Isle_V3 = model.Location_Isle_V3;
            Rotation_Isle_V3 = model.Rotation_Isle_V3;
            Growth = model.Growth;
            Hunger = model.Hunger;
            Thirst = model.Thirst;
            Stamina = model.Stamina;
            Health = model.Health;
            BleedingRate = model.BleedingRate;
            Oxygen = model.Oxygen;
            bGender = model.bGender;
            bIsResting = model.bIsResting;
            bBrokenLegs = model.bBrokenLegs;
            ProgressionPoints = model.ProgressionPoints;
            ProgressionTier = model.ProgressionTier;
            UnlockedCharacters = model.UnlockedCharacters;
            SkinPaletteSection1 = model.SkinPaletteSection1;
            SkinPaletteSection2 = model.SkinPaletteSection2;
            SkinPaletteSection3 = model.SkinPaletteSection3;
            SkinPaletteSection4 = model.SkinPaletteSection4;
            SkinPaletteSection5 = model.SkinPaletteSection5;
            SkinPaletteSection6 = model.SkinPaletteSection6;
            SkinPaletteVariation = model.SkinPaletteVariation;
        }
        public void UpdeteFromDino(DinoModel model)
        {
            Id = model.Id;
            Server = model.Server;
        }

        public void ResponsePrepair()
        {
            Hunger = null;
            Thirst = null;
            Stamina = null;
            Health = null;
            DNA = null;
            //Location_Isle_V3 = null;
            Rotation_Isle_V3 = null;
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
