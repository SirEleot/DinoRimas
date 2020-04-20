using DinoRimas.Data;
using DinoRimas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ServerQueryInfo;
using ServerQueryInfo.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoRimas.Services
{
    public class ServerQueryService
    {
        SettingsModel _settings;
        DateTime _lastCheck;
        Dictionary<string,InfoResponse> _serverinfo;
        public int TotalPlayers { get; set; }
        public Dictionary<string, InfoResponse> Serverinfo {
            get {
                if (_lastCheck < DateTime.Now) GetInfo();
                return _serverinfo;
            } 
        }
        public ServerQueryService(IOptions<SettingsModel> settings)
        {
            _lastCheck = DateTime.Now;
            _settings = settings.Value;
            _serverinfo = new Dictionary<string, InfoResponse>();
        }

        void GetInfo()
        {
            if(_serverinfo.Count > 0) _serverinfo.Clear();
            TotalPlayers = 0;
            foreach (var port in _settings.QueryPosrts)
            {
                try
                {
                    var conn = new QueryConnection();
                    conn.Host = _settings.ServerIp;
                    conn.Port = port.Value;
                    conn.Connect(500);
                    var info = conn.GetInfo();
                    TotalPlayers += info.Players;
                    _serverinfo.Add(port.Key, info);
                }
                catch (Exception)
                {
                    //throw new Exception(e.Message);
                    _serverinfo.Add(port.Key,null);
                }
            }
            if(!_serverinfo.Any(s=>s.Value == null)) _lastCheck = DateTime.Now.AddMinutes(2);
        }
    }
}
