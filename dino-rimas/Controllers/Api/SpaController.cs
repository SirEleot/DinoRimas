using System;
using System.Linq;
using System.Threading.Tasks;
using DinoRimas.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DinoRimas.Extensions;
using DinoRimas.Services;
using DinoRimas.Models;
using Microsoft.Extensions.Options;
using DinoRimas.Settings;

namespace DinoRimas.Controllers.Api
{
    public class Response
    {
        public bool Error { get; set; }
        public string Message { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class SpaController : ControllerBase
    {
        private readonly DinoRimasDbContext _context;
        private readonly UserService _user;
        private readonly SettingsModel _settings;
        public SpaController(DinoRimasDbContext context, UserService user, IOptions<SettingsModel> settings)
        {
            _context = context;
            _user = user;
            _settings = settings.Value;
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(){
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            else
            {
                user.Inventory.RemoveAll(d => d.Server != user.Server);
                if(user.Inventory.Count > 0)
                    foreach (var dino in user.Inventory)
                        dino.ResponsePrepair();
                return Ok(user);
            }
        }

        [HttpGet("GetPrice")]
        public async Task<IActionResult> GetPrice()
        {
            return Ok(_settings.Price);
        }

        [HttpGet("SetPosition")]
        public async Task<IActionResult> SetPosition()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if(user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(!HttpContext.Request.Query.ContainsKey("pos")) return Ok(new Response { Error = true, Message = "Неверно переданы данные." });
            var posId = Convert.ToInt32(HttpContext.Request.Query["pos"]);

            if (user.Balance < _settings.Price.Position) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Position;

            dino.Location_Isle_V3 = Settings.ShopSettings.GetPositionById(posId);
            if (dino.Active)
            {
                var counter = 0;
                while (counter < 2 || !_settings.UpdateSaveFile(user, dino))
                {
                    counter++;
                    await Task.Delay(100);
                };
            }

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Смена позиции для динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно телепортировали вашего динозавра" });
        }

        [HttpGet("ActivateDino")]
        public async Task<IActionResult> ActivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var TargetDino = user.Inventory.SingleOrDefault(d => d.Id == id && d.Server == user.Server);
            if (TargetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });           

            var currentDino = _settings.GetSaveFile(user);
            if (currentDino == null)
            {
                TargetDino.Active = true;
                var counter = 0;
                _context.SaveChanges();
                while (counter < 2 || !_settings.UpdateSaveFile(user, TargetDino, TargetDino.Server))
                {
                    counter++;
                    await Task.Delay(100);
                };
            }
            else
            {
                if (TargetDino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр уже активирован" });
                if (currentDino.Id != default)
                {
                    var dino = user.Inventory.SingleOrDefault(d => d.Id == currentDino.Id);
                    if (dino != null)
                    {
                        dino.Active = false;
                        dino.UpdateFromGame(currentDino);
                    }
                }
                else user.Inventory.Add(currentDino);
                currentDino.Active = false;
                TargetDino.Active = true;
                TargetDino.Deactivated = false;
                _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Активация динозавра {TargetDino.Name} с ID: {TargetDino.Id}"));
                _context.SaveChanges();
                var counter = 0;
                while (counter < 3 || !_settings.UpdateSaveFile(user, TargetDino))
                {
                    counter++;
                    await Task.Delay(100);
                };
            }
           
            return Ok(new Response { Error = false, Message = "Вы успешно активировали динозавра" });
        }

        [HttpGet("AddSlot")]
        public async Task<IActionResult> AddSlot()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });

            if (user.Balance < _settings.Price.Slot) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Slot;
            user.Slot++;

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, "Добавление слота инвентаря"));

            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно добавили дополнительный слот" });
        }

        [HttpGet("SelectServer")]
        public async Task<IActionResult> SelectServer()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            if (_settings.GameSaveFolderPath.Count <= id) return Ok(new Response { Error = true, Message = "Неверно указан Id сервера" });
            user.Server = id;
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = $"Вы перешли к серверу №{user.Server + 1}" });
        }

        [HttpGet("ChangeSex")]
        public async Task<IActionResult> ChangeSex()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var targetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (targetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(targetDino.bGender) return Ok(new Response { Error = true, Message = "Ваш динозавр уже женского пола." });

            var shop = ShopSettings.GetShopDinoByClass(targetDino.CharacterClass);
            var price = (shop != null && !shop.Survival) ? _settings.Price.Sex * 2 : _settings.Price.Sex;
            if (user.Balance < price) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= price;
            targetDino.bGender = true;
            if(targetDino.Active)
            {
                var currentDino = _settings.GetSaveFile(user, targetDino.Server);
                if (currentDino != null)
                {
                    var counter = 0;
                    while (counter < 3 || !_settings.UpdateSaveFile(user, targetDino))
                    {
                        counter++;
                        await Task.Delay(100);
                    };
                }
                else targetDino.Active = false;
            }            
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Смена пола для динозавра {targetDino.Name} с ID: {targetDino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно сменили пол динозавра" });
        }


        [HttpGet("DeleteDino")]
        public async Task<IActionResult> DeleteDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (dino.Active)
            {
                var currentDino = _settings.GetSaveFile(user, dino.Server);
                if (currentDino != null)
                {
                    var counter = 0;
                    while (counter < 3 || !_settings.DeleteSaveFile(user))
                    {
                        counter++;
                        await Task.Delay(100);
                    };
                }
                else dino.Active = false;
            }
            user.Inventory.Remove(dino);
            dino.DNA = $"{user.ProfileName}({user.Steamid}) удален";
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Удаление динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы удалили вашего динозавра и освободили слот" });
        }

        [HttpGet("DisactivateDino")]
        public async Task<IActionResult> DisactivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (user.DeactivaionTime != null && user.DeactivaionTime > DateTime.Now) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе активации. Нужно немного подождать." });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (!dino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр не активен" });
            if (user.Inventory.Where(d=>d.Server == user.Server).ToList().Count >= user.Slot) return Ok(new Response { Error = true, Message = "У вас нет свободных слотов" });           

            var currentDino = _settings.GetSaveFile(user, dino.Server);
            if(currentDino == null ) return Ok(new Response { Error = true, Message = "Этот динозавр не является активным" });
                user.DeactivaionTime = DateTime.Now.AddMinutes(6);
            
            var counter = 0;
            while (counter < 4 || !_settings.DeleteSaveFile(user))
            {
                counter++;
                await Task.Delay(100);
            };
            dino.Active = false;
            dino.Deactivated = true;
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Деактивация динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Для завершения деактивации данного динозавра необходимо подождать 6 мин. Не заходите в игру в течении данного времени" });
        }
       

        private bool CheckProgress(string current, string active)
        {
            if (current == active) return false;
            var _previouse = ShopSettings.hasSub(current) ? "SubS" : "JuvS";
            var className = current[0..^6];
            return (className + _previouse != active);
        }
       
    }
}