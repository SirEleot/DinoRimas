using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class DinoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public bool IsActivated { get; set; }
        public UserModel Owner { get; set; }
        public DinoSaveModel Config { get; set; }
        public DateTime CraetionAs { get; set; }
    }
}
