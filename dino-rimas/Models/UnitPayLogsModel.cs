using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class UnitPayLogsModel
    {
        public static UnitPayLogsModel Create(IQueryCollection q, bool error = false, string message = "")
        {
            //throw new Exception(q["params[orderSum]"].ToString());
            return new UnitPayLogsModel
            {
                UId = Convert.ToInt32(q["params[unitpayId]"].ToString()),
                SteamId = q["params[account]"].ToString(),
                ProfitSum = Convert.ToDouble(q["params[profit]"].ToString().Replace('.', ',')),
                OrderSum = Convert.ToDouble(q["params[orderSum]"].ToString().Replace('.',',')),
                PaymentDate = q["params[date]"].ToString(),
                Compleeted = false,
                Error = error,
                ErrorMessage = q["params[errorMessage]"].ToString() + message,
                CreatedAt = DateTime.Now,
            };
        }
        public int Id { get; set; }
        public int UId { get; set; }
        public string SteamId { get; set; }
        public double ProfitSum { get; set; }
        public double OrderSum { get; set; }
        public string PaymentDate { get; set; }
        public bool Compleeted { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
