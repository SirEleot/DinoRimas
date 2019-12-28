using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoRimas.Models
{
    public class UserModel
    {
        bool _isAdmin;
        public int Id { get; set; }
        public string Steamid { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImg { get; set; }
        public int Balance { get; set; }
        public int Slots { get; set; }
        public bool IsAdmin
        {
            get
            {
                return Steamid == "76561198208390417" ? true : _isAdmin;
            }
            set
            {
                _isAdmin = value;
            }
        }
        public List<DinoModel> Inventory { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
