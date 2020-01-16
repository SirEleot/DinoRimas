using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class DonateShopLogsModel
    {
        public DonateShopLogsModel()
        {

        }
        public DonateShopLogsModel(UserModel user, string log)
        {
            UserProfileName = user.ProfileName;
            UserSteamId = user.Steamid;
            Log = log;
            Date = DateTime.Now;
        }
        public int Id { get; set; }
        public string UserSteamId { get; set; }
        public string UserProfileName { get; set; }
        public string Log { get; set; }
        public DateTime Date { get; set; }
    }
}
