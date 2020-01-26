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
                //UpdateInfo(user);
                user.Inventory.RemoveAll(d => d.Server != user.Server);
                if(user.Inventory.Where(d => d.Server == user.Server).ToList().Count > 0)
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
            //UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(!HttpContext.Request.Query.ContainsKey("pos")) return Ok(new Response { Error = true, Message = "Неверно переданы данные." });
            var posId = Convert.ToInt32(HttpContext.Request.Query["pos"]);

            if (user.Balance < _settings.Price.Position) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Position;

            dino.Location_Isle_V3 = Settings.ShopSettings.GetPositionById(posId);
            if (dino.Active) _settings.UpdateSaveFile(user, dino);

            //_context.DinoModels.Update(dino);
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Смена позиции для динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно телепортировали вашего динозавра" });
        }

        [HttpGet("ActivateDino")]
        public async Task<IActionResult> ActivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            //UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var TargetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (TargetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });           

            var currentDino = _settings.GetSaveFile(user);
            if (currentDino == null)
            {
                TargetDino.Active = true;
                _settings.UpdateSaveFile(user, TargetDino);
                //_context.DinoModels.Update(TargetDino);
            }
            else
            {
                if (TargetDino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр уже активирован" });
                if (currentDino.Id > 0)
                {
                    var dino = user.Inventory.SingleOrDefault(d => d.Id == currentDino.Id);
                    if (dino != null)
                    {
                        dino.Active = false;
                        dino.UpdateFrom(currentDino);
                        //_context.DinoModels.Update(dino);
                    }
                }
                else if (user.Slot > user.Inventory.Where(d => d.Server == user.Server).ToList().Count)
                {
                    user.Inventory.Add(currentDino);
                    _context.Users.Update(user);
                }
                TargetDino.Active = true;
                //_context.DinoModels.Update(TargetDino);
                _settings.UpdateSaveFile(user, TargetDino);
            }

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Активация динозавра {TargetDino.Name} с ID: {TargetDino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно активировали динозавра" });
        }

        [HttpGet("AddSlot")]
        public async Task<IActionResult> AddSlot()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();

            if (user.Balance < _settings.Price.Slot) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Slot;
            user.Slot++;

            //_context.Users.Update(user);
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, "Добавление слота инвентаря"));

            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы успешно добавили дополнительный слот" });
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
            return Ok(new Response { Error = false, Message = $"Вы перешли к серверу №{user.Server + 1}" });
        }

        [HttpGet("ChangeSex")]
        public async Task<IActionResult> ChangeSex()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            //UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var targetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (targetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(targetDino.bGender) return Ok(new Response { Error = true, Message = "Ваш динозавр уже женского пола." });

            var shop = ShopSettings.GetShopDinoByClass(targetDino.CharacterClass);
            var price = (shop != null && !shop.Survival) ? _settings.Price.Sex * 2 : _settings.Price.Sex;
            if (user.Balance < price) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= price;

            var currentDino = _settings.GetSaveFile(user);
            if (currentDino != null && targetDino.Id == currentDino.Id)
            {
                targetDino.UpdateFrom(currentDino);
                targetDino.bGender = true;
                _settings.UpdateSaveFile(user, targetDino);
               // _context.DinoModels.Update(targetDino);
            }
            else
            {
                targetDino.bGender = true;
                //_context.DinoModels.Update(targetDino);
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

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            var currentDino = _settings.GetSaveFile(user);
            if (currentDino != null && dino.Id == currentDino.Id) {
                _settings.DeleteSaveFile(user);
            }

            if(user.Inventory.Contains(dino)) user.Inventory.Remove(dino);
            dino.DNA = $"{user.ProfileName}({user.Steamid}) удален";
            //_context.Users.Update(user);
            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Удаление динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы удалили вашего динозавра и освободили слот" });
        }

        [HttpGet("DisactivateDino")]
        public async Task<IActionResult> DisactivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();

            //UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (!dino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр не активен" });
            if (user.Inventory.Where(d=>d.Server == user.Server).ToList().Count >= user.Slot) return Ok(new Response { Error = true, Message = "У вас нет свободных слотов" });

            var currentDino = _settings.GetSaveFile(user);

            _settings.DeleteSaveFile(user);
            dino.Active = false;
            //_context.DinoModels.Update(dino);

            _context.DonateShopLogs.Add(new DonateShopLogsModel(user, $"Деактивация динозавра {dino.Name} с ID: {dino.Id}"));
            _context.SaveChanges();
            return Ok(new Response { Error = false, Message = "Вы деактивировали вашего динозавра и освободили слот" });
        }


        private void UpdateInfo(UserModel user)
        {
            var activeDino = user.Inventory.FirstOrDefault(d => d.Active && d.Server == user.Server);
            var currentDino = _settings.GetSaveFile(user);
            if(currentDino == null)
            {
                if(activeDino != null)
                {
                    activeDino.Active = false;
                    //_context.DinoModels.Update(activeDino);
                    user.Inventory.Remove(activeDino);
                    activeDino.DNA = $"{user.ProfileName}({user.Steamid}) умер";
                    //_context.Users.Update(user);
                    _context.SaveChanges();
                }
            }
            else
            {
                if(activeDino == null)
                {
                    var dino = user.Inventory.FirstOrDefault(d => d.Id == currentDino.Id && d.Server == user.Server);
                    if (dino == null)
                    {
                        if (currentDino.CharacterClass.Contains("JuvS") || currentDino.CharacterClass.Contains("HatchS") || user.Inventory.Where(d => d.Server == user.Server).ToList().Count == 0)
                        {
                            if (currentDino.Id > 0) currentDino.Id = default;
                            currentDino.Active = true;
                            currentDino.Server = user.Server;
                            user.Inventory.Add(currentDino);
                            //_context.Users.Update(user);
                            _context.SaveChanges();
                            _settings.UpdateSaveFile(user, currentDino);
                        }
                        else
                        {
                            _settings.DeleteSaveFile(user);
                            user.ChangeOnServer++;
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        dino.Active = true;
                       // _context.DinoModels.Update(dino);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    if (currentDino.Id != activeDino.Id)
                    {
                        var dino = user.Inventory.FirstOrDefault(d => d.Id == currentDino.Id && d.Server == user.Server);
                        if (dino == null)
                        {
                            activeDino.Active = false;
                            currentDino.Active = true;
                            user.Inventory.Remove(activeDino);
                            activeDino.DNA = $"{user.ProfileName}({user.Steamid}) умер";
                            if (currentDino.Id > 0) currentDino.Id = default;
                            currentDino.Server = user.Server;
                            user.Inventory.Add(currentDino);
                            //_context.Users.Update(user);
                            _context.SaveChanges();
                            _settings.UpdateSaveFile(user, currentDino);
                            _context.SaveChanges();
                        }
                        else
                        {
                            activeDino.Active = false;
                            dino.Active = true;
                            user.ChangeOnServer += 1;
                            //_context.Users.Update(user);
                            //_context.DinoModels.Update(dino);
                            //_context.DinoModels.Update(activeDino);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        if(currentDino.CharacterClass.Contains("AdultS") && CheckProgress(currentDino.CharacterClass, activeDino.CharacterClass))
                        {
                            _settings.UpdateSaveFile(user, activeDino);
                            user.ChangeOnServer++;
                            //_context.Users.Update(user);
                            _context.SaveChanges();
                        }
                        else
                        {
                            activeDino.UpdateFrom(currentDino);
                            //_context.DinoModels.Update(activeDino);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }

        private bool CheckProgress(string current, string active)
        {
            if (current == active) return false;
            var _previouse = Settings.ShopSettings.hasSub(current) ? "SubS" : "JuvS";
            var className = current[0..^6];
            return (className + _previouse != active);
        }
    }
}