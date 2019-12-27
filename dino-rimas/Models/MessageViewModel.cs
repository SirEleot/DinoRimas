using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinoRimas.Models
{
    public class MessageViewModel
    {
        public MessageViewModel(string tittle, string subtittle, string messsage, string type)
        {
            Tittle = tittle;
            Subtittle = subtittle;
            Message = messsage;
            Type = type;
        }

        public string Tittle { get; set; }
        public string Subtittle { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}
