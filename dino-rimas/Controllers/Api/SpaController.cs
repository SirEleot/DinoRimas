using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinoRimas.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DinoRimas.Extensions;
using Newtonsoft.Json;

namespace DinoRimas.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaController : ControllerBase
    {
        private readonly DinoRimasDbContext _context;

        public SpaController(DinoRimasDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(){
            var user = await User.GetDinoUserAsync();
            if (user == null) return NotFound();
            return Content(JsonConvert.SerializeObject(user));
        }

        [HttpGet("AddSlot")]
        public async Task <IActionResult> AddUserSlot()
        {
            var user = await User.GetDinoUserAsync();
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