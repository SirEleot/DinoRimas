using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using DinoRimas.Models;
using Newtonsoft.Json;
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
        public DonateController(DinoRimasDbContext context, IWebHostEnvironment env)
        {
            _context = context;
        }

        [HttpGet("Validate")]
        public IActionResult ValidatePayment()
        { 
            if (Program.Settings.UnitPay.IpList.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                var query = HttpContext.Request.Query;
                //if(query["params[test]"] == "1") return Ok(new UnitPayOk("Тестовый запрос получен").ToString());
                switch (query["method"])
                {
                    case "check": return Ok(new UnitPayOk("Сервер готов").ToString());
                    case "pay":
                        var remoteSign = query["params[signature]"];
                        var localSign = GetSign(query);
                        if(remoteSign == localSign)
                        {
                            var u = UnitPayLogsModel.Create(query, false);
                            if (_context.UnitPayLogs.Any(up=>up.UId == u.UId && up.Compleeted == true)) return Ok(new UnitPayOk("Платеж с таким ИД уже существует").ToString());
                            var user = _context.User.Where(usr => usr.Steamid == u.SteamId).SingleOrDefault();
                            if(user == null)
                            {
                                u.Error = true;
                                u.ErrorMessage = $"Пользователя с данным steamid нет в базе";
                            }
                            else
                            {
                                user.Balance += (int)u.ProfitSum;
                                _context.Update(user);
                                _context.SaveChangesAsync();
                                u.Compleeted = true;
                            }

                            _context.UnitPayLogs.Add(u);
                            _context.SaveChanges();
                            return Ok(new UnitPayOk("Платеж успешно обработан").ToString());
                        }else return Ok(new UnitPayError("Не прошел проверку, пожалуйста сообщите администратору").ToString());
                        
                    case "error":
                        {
                            var u = UnitPayLogsModel.Create(query, true); 
                            _context.UnitPayLogs.Add(u);
                            _context.SaveChanges();
                            return Ok(new UnitPayOk("Сообщение об ошибке получено"));
                        }
                    default: return Ok(new UnitPayError("Неизвестный метод запроса, пожалуйста сообщите администратору").ToString());
                }
            }
            else
            {
                return Ok(new UnitPayError("Доступ запрещен, пожалуйста сообщите администратору").ToString());
            }
        }

        private string GetSign(IQueryCollection q)
        {
            var key = q.Where( k=> k.Key != "params[sign]" && k.Key != "params[signature]").OrderBy(s=>s.Key).Aggregate("", (s,i) => s += i.Value + "{up}");            
            key += Program.Settings.UnitPay.PrivateKey;
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