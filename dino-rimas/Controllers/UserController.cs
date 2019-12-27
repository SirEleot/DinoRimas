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
using DinoRimas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DinoRimas.Extensions;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace DinoRimas.Controllers
{
    public class UserController : Controller
    {
        private readonly DinoRimasDbContext _context;

        public UserController(DinoRimasDbContext context)
        {
            _context = context;       
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
            var user = await User.GetDinoUserAsync();
            ViewData["Account"] = user?.Steamid ?? "";
            return View();
        }
    }
}
