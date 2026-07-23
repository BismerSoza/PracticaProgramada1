using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace PracticaProgramada1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public HomeController(IHttpClientFactory http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UsuarioModel model)
        {
            using var client = _http.CreateClient();
            var urlApi = _config["Valores:UrlApi"] + "Login/login"; // 🔑 api/Login/login

            var response = await client.PostAsJsonAsync(urlApi, model);

            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                return RedirectToAction("Dashboard", "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ViewBag.Error = "Credenciales incorrectas o usuario inactivo";
                return View(model);
            }
            else
            {
                ViewBag.Error = "Error al conectar con el servidor";
                return View(model);
            }
        }

        public IActionResult Dashboard() => View();
        public IActionResult GeneralDashboard() => View();
        public IActionResult LayoutDefault() => View();

        [HttpGet]
        public IActionResult Registro() => View();

        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioModel model)
        {
            using var client = _http.CreateClient();
            var urlApi = _config["Valores:UrlApi"] + "Registro/crear";

            var response = await client.PostAsJsonAsync(urlApi, model);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Usuario registrado correctamente";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Error al registrar usuario";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Recuperar() => View();
    }
}
