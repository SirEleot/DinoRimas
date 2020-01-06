using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinoRimas.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DinoRimas.Extensions;
using DinoRimas.Services;
using DinoRimas.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
                var file = _settings.GetSaveFile(user);
                var active = user.Inventory.SingleOrDefault(d => d.IsActivated && d.IsAlive);

                if(active != null)  //если в инвентаре есть активный динозавр
                {
                    if (file == null)   //если в папке с игрой нет файла с динозавром
                    {
                        active.IsActivated = false; //помечаем неактивным
                        active.IsAlive = false; //мертвым
                        _context.User.Update(user); //обновляем информацию в базе
                        _context.SaveChanges();
                        user.Inventory.Remove(active); //удаляем из инвентаря
                    }
                    else if(file.CharacterClass != active.Config.CharacterClass) //если файл в папке с игрой есть но его класс не совпадает с классом активного дино
                    {
                        active.IsActivated = false; //неактивным
                        active.IsAlive = false; //мертвым
                        user.Inventory.Add( //создаем нового дино из файла и добавляем в инвентарь как активного
                                 _user.CreateNewDino(file, true)
                            );
                        _context.User.Update(user); //обновляем
                        _context.SaveChanges();
                        user.Inventory.Remove(active); //удаляем из инвентаря
                    }
                }
                else if(file != null && user.Slots > user.Inventory.Count) //если активных дино в инвентаре не обноружено, но есть файл в пвпке с игрой и есть свободные слоты
                {
                    user.Inventory.Add(
                        _user.CreateNewDino(file, true)
                      );
                    _context.User.Update(user);
                    _context.SaveChanges();
                }
                return Ok(user);
            }
        }


        [HttpGet("SetPosition")]
        public async Task<IActionResult> SetPosition()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (_user.UserOnServer(user.Steamid)) return Ok(new Response { Error = true, Message = "Покиньте сервер"});
            if(user.Balance < _settings.Price.Position) return Ok(new Response { Error = true, Message = "Недостаточно средств" });

            return Ok(new Response { Error = false, Message = "Вы успешно телепортировали вашего динозавра" });
        }

        [HttpGet("ActivateDino")]
        public async Task<IActionResult> ActivateDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (_user.UserOnServer(user.Steamid)) return Ok(new Response { Error = true, Message = "Покиньте сервер" });
            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id && !d.IsActivated);
            if(dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            var save = _settings.GetSaveFile(user); 
            if (save != null)
            {
                var activeDino = user.Inventory.SingleOrDefault(d => d.IsActivated);
                if(activeDino != null && activeDino.Config.CharacterClass == save.CharacterClass) 
                {
                    activeDino.Config = save;
                    activeDino.IsActivated = false;
                }
            }
            _settings.AddSaveFile(user, dino.Config);
            dino.IsActivated = true;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new Response { Error = false, Message = "Вы успешно активировали динозавра" });
        }

        [HttpGet("AddSlot")]
        public async Task<IActionResult> AddSlot()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (_user.UserOnServer(user.Steamid)) return Ok(new Response { Error = true, Message = "Покиньте сервер" });
            if (user.Balance < _settings.Price.Slot) return Ok(new Response { Error = true, Message = "Недостаточно средств" });
            user.Balance -= _settings.Price.Slot;
            user.Slots++;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new Response { Error = false, Message = "Вы успешно добавили дополнительный слот" });
        }

        [HttpGet("ChangeSex")]
        public async Task<IActionResult> ChangeSex()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (_user.UserOnServer(user.Steamid)) return Ok(new Response { Error = true, Message = "Покиньте сервер" });
            if (user.Balance < _settings.Price.Sex) return Ok(new Response { Error = true, Message = "Недостаточно средств" });

            return Ok(new Response { Error = false, Message = "Вы успешно сменили пол динозавра" });
        }

        [HttpGet("DeleteDino")]
        public async Task<IActionResult> DeleteDino()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            if (_user.UserOnServer(user.Steamid)) return Ok(new Response { Error = true, Message = "Покиньте сервер" });

            var id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var dino = user.Inventory.SingleOrDefault(d => d.Id == id);
            if (dino == null) return Ok(new Response { Error = true, Message = "Динозавр с таким Id не найден" });
            if (dino.IsActivated) _settings.DeleteSaveFile(user);
            user.Inventory.Remove(dino);
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new Response { Error = false, Message = "Вы удалили вашего динозавра и освободили слот" });
        }


    }
}