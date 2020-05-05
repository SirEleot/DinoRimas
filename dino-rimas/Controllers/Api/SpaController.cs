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
using DinoRimas.FileWatcher;
using System.Collections.Generic;

namespace DinoRimas.Controllers.Api
{
    public class Response
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public UserModel User { get; set; } = null;
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
                return Ok(UpdateInventory(user));
            }
        }

        private UserModel UpdateInventory(UserModel user)
        {
            user.Inventory.RemoveAll(d => d.Server != user.Server);
            if (user.Inventory.Count > 0)
                foreach (var dino in user.Inventory)
                    dino.ResponsePrepair();
            return user;
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
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if(dino.Active) return Ok(new Response { Error = true, Message = "Все действия необходимо производить с деактивированым динозавром" });
            if (!HttpContext.Request.Query.ContainsKey("pos")) return Ok(new Response { Error = true, Message = "Неверно переданы данные." });
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
            return Ok(new Response { Error = false, Message = "Вы успешно телепортировали вашего динозавра", User = UpdateInventory(user) });
        }

        [HttpGet("ActivateDino")]
        public async Task<IActionResult> ActivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var TargetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (TargetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (TargetDino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр уже активирован" });
            if (user.Inventory.Any(d=>d.Active && d.Server == user.Server)) return Ok(new Response { Error = true, Message = "Для активации нового динозавра нужно деактивировать старого." });
           
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
            
            return Ok(new Response { Error = false, Message = "Вы успешно активировали динозавра", User = UpdateInventory(user) });
        }

        [HttpGet("AddSlot")]
        public async Task<IActionResult> AddSlot()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();

            if (user.Balance < _settings.Price.Slot) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Slot;
            user.Slot++;

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, "Добавление слота инвентаря"));

            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно добавили дополнительный слот", User = UpdateInventory(user) });
        }

        [HttpGet("SelectServer")]
        public async Task<IActionResult> SelectServer()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            if (_settings.GameSaveFolderPath.Count <= id) return Ok(new Response { Error = true, Message = "Неверно указан Id сервера" });
            user.Server = id;
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = $"Вы перешли к серверу №{user.Server + 1}", User = UpdateInventory(user) });
        }

        [HttpGet("ChangeSex")]
        public async Task<IActionResult> ChangeSex()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var targetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (targetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (targetDino.Active) return Ok(new Response { Error = true, Message = "Все действия необходимо производить с деактивированым динозавром" });

            if (targetDino.bGender) return Ok(new Response { Error = true, Message = "Ваш динозавр уже женского пола." });

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
            return Ok(new Response { Error = false, Message = "Вы успешно сменили пол динозавра", User = UpdateInventory(user) });
        }

        List<string> _hasSub = new List<string>
        {
           "Rex",
           "Trike",
           "Giga"
        };

        [HttpGet("GrowDino")]
        public async Task<IActionResult> GrowDino()
        {

            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var targetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (targetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (targetDino.Active) return Ok(new Response { Error = true, Message = "Все действия необходимо производить с деактивированым динозавром" });

            var price = _settings.Price.Grow;
            if (user.Balance < price) return Ok(new Response { Error = true, Message = "Недостаточно средств" });

            int stage; 
            var newClassName = GetNextClassName(targetDino.CharacterClass, out stage);
            if (stage < 0) return Ok(new Response { Error = true, Message = "Ваш динозавр уже максимального уровня." });
            user.Balance -= price;
            targetDino.CharacterClass = newClassName;

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Grow dino {targetDino.Name} с ID: {targetDino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно повысили динозавра", User = UpdateInventory(user) });
        }

        private int GetStageFromClass(string className)
        {
            return _settings.Stages.FindIndex(s => className.Contains(s));
        }
        private string GetNextClassName(string className, out int oldIndex)
        {
            oldIndex = GetStageFromClass(className);
            if (oldIndex == (_settings.Stages.Count - 1)) oldIndex = -1;
            if (oldIndex < 0) return "";
            var isSurv= className[className.Length - 1] == 'S';
            var letters = isSurv ?  _settings.Stages[oldIndex].Length + 1 : _settings.Stages[oldIndex].Length;
            return className[0..^letters] + _settings.Stages[oldIndex + 1] + (isSurv ? "S" : "");
        }

        [HttpGet("DeleteDino")]
        public async Task<IActionResult> DeleteDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (dino.Active) return Ok(new Response { Error = true, Message = "Все действия необходимо производить с деактивированым динозавром" });

            user.Inventory.Remove(dino);
            dino.DNA = $"{user.ProfileName}({user.Steamid}) удален";

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Удаление динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы удалили вашего динозавра и освободили слот", User = UpdateInventory(user) });
        }

        [HttpGet("DisactivateDino")]
        public async Task<IActionResult> DisactivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (DinoWatcher.DeactivateBeginned(user.Steamid)) return Ok(new Response { Error = true, Message = "Один из динозавров находится в процессе дактивации или удаления. Нужно немного подождать." });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (!dino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр не активен" });
            if (user.Inventory.Where(d=>d.Server == user.Server).ToList().Count >= user.Slot) return Ok(new Response { Error = true, Message = "У вас нет свободных слотов" });           

            var currentDino = _settings.GetSaveFile(user, dino.Server);
            if(currentDino == null ) return Ok(new Response { Error = true, Message = "Этот динозавр не является активным" });

            var path = _settings.GameSaveFolderPath[dino.Server] + @"\" + user.Steamid + ".json";
            DinoWatcher.AddToDeactivateQueue(user.Steamid, dino.Server);

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Запрос на деактивацию динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = $"Для завершения деактивации данного динозавра необходимо подождать {_settings.DeactivationTime} мин. Не заходите в игру до оконяания таймера", User = UpdateInventory(user) });
        }
       
    }
}