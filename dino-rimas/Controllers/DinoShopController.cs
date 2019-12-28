using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DinoRimas.Data;
using DinoRimas.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using DinoRimas.Services;
using Microsoft.Extensions.Options;
using DinoRimas.Extensions;

namespace DinoRimas.Controllers
{
    public class DinoShopController : Controller
    {
        private readonly DinoRimasDbContext _context;
        private readonly UserService _user;
        private readonly SettingsModel _settings;

        public DinoShopController(DinoRimasDbContext context, UserService user, IOptions<SettingsModel> settings)
        {
            _context = context;
            _user = user;
            _settings = settings.Value;
        }

        // GET: ShopItems
        public async Task<IActionResult> Index()
        {
            Tuple<UserModel, List<DinoShopModel>> model = new Tuple<UserModel, List<DinoShopModel>>(await _user.GetDinoUserAsync(), await _context.ShopDinoList.ToListAsync());
            return View(model);
        }

        public async Task<IActionResult> BuyDino(int id)
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null) return View("Message", new MessageViewModel(
                "Ошибка",
                "Приносим свои извинения!",
                "Пользователь с таким ID в базе не обгаружен. Обратитесь к администрации",
                "error"
                ));

            if(user.Inventory?.Count >= user.Slots) return View("Message", new MessageViewModel(
               "Ошибка",
               "Невозможно завершить операцию!",
               "У вас не хватает слотов для динозавров в инвентаре. Удалите какого-нибудь динозавра или приобретите дополнительный слот.",
               "error"
               ));

            var dino = await _context.ShopDinoList.FirstOrDefaultAsync(d=>d.Id == id);
            if(dino == null) return View("Message", new MessageViewModel(
                "Ошибка",
                "Приносим свои извинения!",
                "Динозавр не найден в базе. Пожалуйста обратитесь к администрации",
                "error"
                ));

            if(dino.Price > user.Balance) return View("Message", new MessageViewModel(
                "Ошибка",
                "Внимание!",
                "У вас недостаточно Dino Coin на счету",
                "error"
                ));

            user.Balance -= dino.Price;
            if (user.Inventory == null) user.Inventory = new List<DinoModel>();
            user.Inventory.Add(_user.CreateNewDino(user, dino));
            _context.User.Update(user);
            _context.SaveChanges();
            
            return View("Message", new MessageViewModel(
                "Поздравляем",
                "Вы приобрели динозавра",
                "Активировать его или провести какие-то дополнительные манипуляции с ним вы сможете на странице личного кабинета",
                "success"
                ));
        }
        public async Task<IActionResult> UpdateDinoList()
        {
            var user = await _user.GetDinoUserAsync();
            if (user == null || !user.IsAdmin) return Redirect("/");

            var folder = "ShopDinoList";
            var files = Directory.GetFiles(folder);
            var dinos = new List<DinoShopModel>();
            foreach (var file in files)
            {
                using StreamReader r = new StreamReader(file, Encoding.UTF8);
                dinos.Add(JsonConvert.DeserializeObject<DinoShopModel>(r.ReadToEnd()));
            }
            foreach (var dino in dinos)
            {
                if(!_context.ShopDinoList.Any(d=>d.Name == dino.Name))
                {
                    _context.ShopDinoList.Add(dino);
                }
            }
            await _context.SaveChangesAsync();
            return View("Message", new MessageViewModel(
              "Поздравляем",
              "Список динозавров обновлен",
              "",
              "success"
              ));
        }
    }
}
