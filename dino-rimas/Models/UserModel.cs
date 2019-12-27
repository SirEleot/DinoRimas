using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoRimas.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Steamid { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImg { get; set; }
        public int Balance { get; set; }
        public int Slots { get; set; }
        public List<DinoModel> Inventory { get; set; }
        public List<DinoModel> BaseDinos { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
