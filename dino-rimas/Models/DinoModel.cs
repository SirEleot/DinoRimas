using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DinoRimas.Models
{
    public class DinoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public bool IsActivated { get; set; }
        public bool Vip { get; set; }
        public DinoSaveModel Config { get; set; }
        public DateTime CraetionAs { get; set; }
    }
}
