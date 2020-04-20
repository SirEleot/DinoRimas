using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using DinoRimas.Models;
using System.Text.Json;
using DinoRimas.Data;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace DinoRimas.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonateController : Controller
    {
        private readonly DinoRimasDbContext _context;
        private readonly SettingsModel _settings;
        public DonateController(DinoRimasDbContext context, IOptions<SettingsModel> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        [HttpGet("Validate")]
        public IActionResult ValidatePayment()
        { 
            if (_settings.UnitPay.IpList.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                var query = HttpContext.Request.Query;
                if(query["params[test]"] == "1") return Ok(new UnitPayOk("Тестовый запрос получен"));
                switch (query["method"])
                {
                    case "check": return Ok(new UnitPayOk("Сервер готов"));
                    case "pay":
                        var remoteSign = query["params[signature]"];
                        var localSign = GetSign(query);
                        if(remoteSign == localSign)
                        {
                            var u = UnitPayLogsModel.Create(query, false);
                            var log = _context.UnitPayLogs.FirstOrDefault(up => up.UId == u.UId);
                            if(log == null)
                            {
                                var user = _context.Users.Where(usr => usr.Steamid == u.SteamId).FirstOrDefault();                                
                                if (user == null)
                                {
                                    u.Error = true;
                                    u.ErrorMessage = $"Пользователя с данным steamid нет в базе";
                                    u.Compleeted = false;
                                }
                                else
                                {
                                    user.Balance += (int)u.ProfitSum;
                                    u.Compleeted = true;
                                }
                                _context.UnitPayLogs.Add(u);
                                _context.SaveChanges();
                                return Ok(new UnitPayOk("Платеж успешно обработан"));
                            }
                            else return Ok(new UnitPayError("Платеж с таким ИД уже существует"));

                        }else return Ok(new UnitPayError("Не прошел проверку, пожалуйста сообщите администратору"));
                        
                    case "error":
                        {
                            var u = UnitPayLogsModel.Create(query, true); 
                            _context.UnitPayLogs.Add(u);
                            _context.SaveChanges();
                            return Ok(new UnitPayOk("Сообщение об ошибке получено"));
                        }
                    default: return Ok(new UnitPayError("Неизвестный метод запроса, пожалуйста сообщите администратору"));
                }
            }
            else
            {
                return Ok(new UnitPayError("Доступ запрещен, пожалуйста сообщите администратору"));
            }
        }

        [HttpGet("redirect")]
        public IActionResult RedirectToPay()
        {
            var query = HttpContext.Request.Query;
            var account = query["account"];
            var sum = query["sum"];
            var desc = query["desc"];
            var currency = query["currency"];
            var url = $"https://unitpay.ru/pay/{_settings.UnitPay.PublickKey}?account={account}&desc={desc}&sum={sum}&currency={currency}&signature={GetRequestSign(account, currency, desc, sum)}";
            return Redirect(url);
        }

        private string GetRequestSign(string account, string currency, string desc, string sum)
        {
            var key = account + "{up}" + currency + "{up}"+ desc + "{up}" + sum + "{up}" + _settings.UnitPay.PrivateKey;
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private string GetSign(IQueryCollection q)
        {
            var key = q.Where( k=> k.Key != "params[sign]" && k.Key != "params[signature]").OrderBy(s=>s.Key).Aggregate("", (s,i) => s += i.Value + "{up}");            
            key += _settings.UnitPay.PrivateKey;
            //throw new Exception(key);
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

    }
}