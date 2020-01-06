using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DinoRimas.Models
{
    public class DinoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsAlive { get; set; }
        public bool IsActivated { get; set; }
        public bool Vip { get; set; }
        [JsonIgnore]
        public DinoSaveModel Config { get; set; }
        public DateTime CraetionAs { get; set; }
    }
}
