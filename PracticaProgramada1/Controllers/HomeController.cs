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

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult GeneralDashboard()
        {
            return View();
        }

        public IActionResult LayoutDefault()
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
