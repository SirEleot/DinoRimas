using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DinoRimas.Data;
using DinoRimas.Models;
using DinoRimas.Services;
using Microsoft.Extensions.Options;
using DinoRimas.Settings;

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
            var model = new Tuple<UserModel, List<DinoShopModel>>(await _user.GetDinoUserAsync(), ShopSettings.GetShopList());
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

            if (user.Inventory?.Where(d => d.Server == user.Server).ToList().Count >= user.Slot) return View("Message", new MessageViewModel(
               "Ошибка",
               "Невозможно завершить операцию!",
               "У вас не хватает слотов для динозавров в инвентаре. Удалите какого-нибудь динозавра или приобретите дополнительный слот.",
               "error"
               ));

            var dinoShop = ShopSettings.GetShopDinoById(id);
            if(dinoShop == null) return View("Message", new MessageViewModel(
                "Ошибка",
                "Приносим свои извинения!",
                "Динозавр не найден в базе. Пожалуйста обратитесь к администрации",
                "error"
                ));

            //if (user.Inventory.Any(d => d.CharacterClass == dinoShop.CharacterClass)) return View("Message", new MessageViewModel(
            //     "Ошибка",
            //     "Невозможно завершить операцию!",
            //     "В инвентаре не может находится 2 одинаковых динозавра.",
            //     "error"
            //     ));

            if (dinoShop.Price > user.Balance) return View("Message", new MessageViewModel(
                "Ошибка",
                "Внимание!",
                "У вас недостаточно Dino Coin на счету",
                "error"
                ));

            user.Balance -= dinoShop.Price;
            if (user.Inventory == null) user.Inventory = new List<DinoModel>();
            var dino = _user.CreateNewDino(dinoShop, user.Server);
            user.Inventory.Add(dino);
            _context.Users.Update(user);
            _context.SaveChanges();
            
            return View("Message", new MessageViewModel(
                "Поздравляем",
                "Вы приобрели динозавра",
                "Активировать его или провести какие-то дополнительные манипуляции с ним вы сможете на странице личного кабинета",
                "success"
                ));
        }
       
    }
}
