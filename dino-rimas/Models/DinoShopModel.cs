using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class DinoShopModel
    {   
        public int Id { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public bool Survival { get; set; }
        public DinoSaveModel BaseConfig { get; set; }
    }   
}
