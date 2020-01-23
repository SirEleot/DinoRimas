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
        List<InfoResponse> _serverinfo;
        public List<InfoResponse> Serverinfo {
            get {
                if (_lastCheck < DateTime.Now) GetInfo();
                return _serverinfo;
            } 
        }
        public ServerQueryService(IOptions<SettingsModel> settings)
        {
            _lastCheck = DateTime.Now;
            _settings = settings.Value;
            _serverinfo = new List<InfoResponse>();
        }

        void GetInfo()
        {
            if(_serverinfo.Count > 0) _serverinfo.Clear();
            foreach (var port in _settings.QueryPosrts)
            {
                try
                {
                    var conn = new QueryConnection();
                    conn.Host = _settings.ServerIp;
                    conn.Port = port;
                    conn.Connect(500);
                    _serverinfo.Add(conn.GetInfo());
                }
                catch (Exception)
                {
                    //throw new Exception(e.Message);
                    _serverinfo.Add(null);
                }
            }
            if(!_serverinfo.Any(s=>s == null)) _lastCheck = DateTime.Now.AddMinutes(2);
        }
    }
}
