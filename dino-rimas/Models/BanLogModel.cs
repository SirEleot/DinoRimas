using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class BanLogModel
    {
        public int Id { get; set; }
        public string Steamid { get; set; }
        public string Reason { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
