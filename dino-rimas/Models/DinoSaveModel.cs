using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class DinoSaveModel
    {
        public string CharacterClass { get; set; }
        public string DNA { get; set; }
        public string Location_Isle_V3 { get; set; }
        public string Rotation_Isle_V3 { get; set; }
        public string Growth { get; set; }
        public string Hunger { get; set; }
        public string Thirst { get; set; }
        public string Stamina { get; set; }
        public string Health { get; set; }
        public string BleedingRate { get; set; }
        public string Oxygen { get; set; }
        public bool bGender { get; set; }
        public bool bIsResting { get; set; }
        public bool bBrokenLegs { get; set; }
        public string ProgressionPoints { get; set; }
        public string ProgressionTier { get; set; }
        public string UnlockedCharacters { get; set; }
        public string CameraRotation_Isle_V3 { get; set; }
        public string CameraDistance_Isle_V3 { get; set; }
        public int SkinPaletteSection1 { get; set; }
        public int SkinPaletteSection2 { get; set; }
        public int SkinPaletteSection3 { get; set; }
        public int SkinPaletteSection4 { get; set; }
        public int SkinPaletteSection5 { get; set; }
        public int SkinPaletteSection6 { get; set; }
        public string SkinPaletteVariation { get; set; }

    }
}
