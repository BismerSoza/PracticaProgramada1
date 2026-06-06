using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace PracticaProgramada1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(UsuarioModel model)
        {
            return View();
        }

    }
}
