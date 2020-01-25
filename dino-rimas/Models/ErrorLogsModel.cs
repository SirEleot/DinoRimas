using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class ErrorLogsModel
    {
        public ErrorLogsModel(string message)
        {
            Message = message;
            Date = DateTime.Now;
        }
        public ErrorLogsModel(){}
        public int Id { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public DateTime Date { get; set; }
    }
}
