using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
 
namespace DinoRimas.Models
{
    public class UnitPayModel
    {
        public string PublickKey { get; set; }
        public string PrivateKey { get; set; }
        public List<string> IpList { get; set; }
    }

    public class UnitPayOk
    {
        public UnitPayOk(string msg)
        {
            result = new Dictionary<string, string> { { "message", msg } };
        }
        public Dictionary<string, string> result { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class UnitPayError
    {
        public UnitPayError(string msg)
        {
            error = new Dictionary<string, string> { { "message", msg } };
        }
        public Dictionary<string, string> error { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
