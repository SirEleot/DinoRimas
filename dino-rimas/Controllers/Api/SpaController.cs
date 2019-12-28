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

                if(active != null)
                {
                    if (file == null)
                    {
                        active.IsActivated = false;
                        active.IsAlive = false;
                        _context.User.Update(user);
                        _context.SaveChanges();
                        user.Inventory.Remove(active);
                    }
                    else if(file.CharacterClass != active.Config.CharacterClass)
                    {
                        active.IsActivated = false;
                        active.IsAlive = false;
                        user.Inventory.Add(
                                 _user.CreateNewDino(user, file)
                            );
                        _context.User.Update(user);
                        _context.SaveChanges();
                        user.Inventory.Remove(active);
                    }
                }
                else if(file != null)
                {
                    user.Inventory.Add(
                        _user.CreateNewDino(user, file)
                      );
                    _context.User.Update(user);
                    _context.SaveChanges();
                }
                return Ok(user);
            }
        }

        [HttpGet("AddSlot")]
        public async Task <IActionResult> AddUserSlot()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return NotFound();
            else 
            {
                user.Slots++;
                _context.User.Update(user);
                await _context.SaveChangesAsync();
                return Ok(user.Slots);
            }
        }
        
    }
}