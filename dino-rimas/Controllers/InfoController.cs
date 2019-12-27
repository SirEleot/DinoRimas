using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DinoRimas.Models;
using DinoRimas.Data;

namespace DinoRimas.Controllers
{
    public class InfoController :Controller
    {
        private readonly ILogger<InfoController> _logger;
        private readonly DinoRimasDbContext _context;

        public InfoController(ILogger<InfoController> logger, DinoRimasDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Regulations()
        {
            return View();
        }
        public IActionResult Help()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            return View(Program.Settings.Contacts);
        }
        public IActionResult SuccessPay()
        {
            return View("Message", new MessageViewModel(
               "Платеж получен",
               "Ваш платеж успешно получен.",
               "Администрация выражает вам благодарность за помощь проекту. Спасибо что учавствуете в поддердании и развитии сервера.",
               "success"
               ));
        }

        public IActionResult ErrorPay()
        {
            return View("Message", new MessageViewModel(
                "Ошибка платежа",
                "Приносим свои извинения!",
                "Во вроемя проведения платежа произошла ошибка и платеж не был завершен. Возможно причиной послужили временные неиспраности в системе. Попробуйте повторить платеж позже.",
                "error"
                ));
        }
    }
}
