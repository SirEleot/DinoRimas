using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DinoRimas.FileWatcher;

namespace DinoRimas.Models
{
    public class UserModel
    {
        bool _isAdmin = false;
        public int Id { get; set; }
        public string Steamid { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImg { get; set; }
        public int Balance { get; set; }
        [NotMapped]
        public int DeactivaionTime { 
            get {
                var time = DinoWatcher.DeactivateTime(Steamid);
                if (time == null) return -1;
                var diff = time.Value - DateTime.UtcNow;
                return diff.Seconds + diff.Minutes * 60; 
            } 
        }
        [JsonIgnore]
        public DateTime? BannedTo { get; set; }
        [NotMapped]
        public bool Banned { 
            get {
                if (BannedTo == null) return false;
                return BannedTo > DateTime.Now;
            } 
        }
        [JsonIgnore]
        public List<int> Slots { get; set; }
        public int Server { get; set; } = 0;
        public int ChangeOnServer { get; set; } = 0;
        [NotMapped]
        public int Slot { 
            get {
                return Slots[Server];
            } 
            set {
                Slots[Server] = value;
            } 
        }
        [JsonIgnore]
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
            return JsonSerializer.Serialize(this);
        }
    }
}
