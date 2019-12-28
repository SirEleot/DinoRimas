using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DinoRimas.Data;
using DinoRimas.Services;

namespace DinoRimas.Controllers
{
    public class UserController : Controller
    {
        private readonly DinoRimasDbContext _context;
        private readonly UserService _user;

        public UserController(DinoRimasDbContext context, UserService user)
        {
            _context = context;
            _user = user;
        }
        // GET: Users
        public async Task<IActionResult> Cabinet()
        {            
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Steam");
        }
        [HttpPost]
        public async Task<IActionResult> SignIn([FromForm] string provider)
        {
            return Challenge(new AuthenticationProperties());
        }
        public IActionResult SignOut()
        {          
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> Donate()
        {
            var user = await _user.GetDinoUserAsync();
            ViewData["Account"] = user?.Steamid ?? "";
            return View();
        }
    }
}
