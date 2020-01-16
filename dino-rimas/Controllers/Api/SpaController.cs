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
                UpdateInfo(user);
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
            UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(!HttpContext.Request.Query.ContainsKey("pos")) return Ok(new Response { Error = true, Message = "Неверно переданы данные." });
            var posId = Convert.ToInt32(HttpContext.Request.Query["pos"]);

            if (user.Balance < _settings.Price.Position) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Position;

            dino.Location_Isle_V3 = Settings.ShopSettings.GetPositionById(posId);
            if (dino.Active) _settings.AddSaveFile(user, dino);

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
            UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var TargetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (TargetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });           

            var currentDino = _settings.GetSaveFile(user);
            if (currentDino == null)
            {
                TargetDino.Active = true;
                _settings.AddSaveFile(user, TargetDino);
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
                else if (user.Slot > user.Inventory.Count)
                {
                    user.Inventory.Add(currentDino);
                    _context.Users.Update(user);
                }
                TargetDino.Active = true;
                //_context.DinoModels.Update(TargetDino);
                _settings.AddSaveFile(user, TargetDino);
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

        [HttpGet("ChangeSex")]
        public async Task<IActionResult> ChangeSex()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var targetDino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (targetDino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });

            if(targetDino.bGender) return Ok(new Response { Error = true, Message = "Ваш динозавр уже женского пола." });

            if (user.Balance < _settings.Price.Sex) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Sex;

            var currentDino = _settings.GetSaveFile(user);
            if (currentDino != null && targetDino.Id == currentDino.Id)
            {
                targetDino.UpdateFrom(currentDino);
                targetDino.bGender = true;
                _settings.AddSaveFile(user, targetDino);
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

            UpdateInfo(user);
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (!dino.Active) return Ok(new Response { Error = true, Message = "Этот динозавр не активен" });
            if (user.Inventory.Count >= user.Slot) return Ok(new Response { Error = true, Message = "У вас нет свободных слотов" });

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
            var activeDino = user.Inventory.FirstOrDefault(d => d.Active);
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
                    var dino = user.Inventory.FirstOrDefault(d => d.Id == currentDino.Id);
                    if (dino == null)
                    {
                        if (currentDino.CharacterClass.Contains("JuvS") || currentDino.CharacterClass.Contains("HatchS"))
                        {
                            user.Inventory.Add(currentDino);
                            if (currentDino.Id > 0) currentDino.Id = default;
                            currentDino.Active = true;
                            //_context.Users.Update(user);
                            _context.SaveChanges();
                            _settings.AddSaveFile(user, currentDino);
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
                        var dino = user.Inventory.FirstOrDefault(d => d.Id == currentDino.Id);
                        if (dino == null)
                        {
                            activeDino.Active = false;
                            currentDino.Active = true;
                            user.Inventory.Remove(activeDino);
                            activeDino.DNA = $"{user.ProfileName}({user.Steamid}) умер";
                            user.Inventory.Add(currentDino);
                            if (currentDino.Id > 0) currentDino.Id = default;
                            //_context.Users.Update(user);
                            _context.SaveChanges();
                            _settings.AddSaveFile(user, currentDino);
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
                            _settings.AddSaveFile(user, activeDino);
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