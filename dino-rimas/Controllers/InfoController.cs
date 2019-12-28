using Microsoft.AspNetCore.Mvc;
using DinoRimas.Models;
using Microsoft.Extensions.Options;

namespace DinoRimas.Controllers
{
    public class InfoController :Controller
    {
        private readonly SettingsModel _settings;

        public InfoController(IOptions<SettingsModel> settings)
        {
            _settings = settings.Value;
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
            return View(_settings.Contacts);
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
