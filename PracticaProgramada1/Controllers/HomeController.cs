using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Models;
using PracticaProgramada1.Filters;
using System.Net;
using System.Text.Json;

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
            try
            {
                using var client = _http.CreateClient();
                var urlApi = _config["Valores:UrlApi"] + "Login/login";

                var response = await client.PostAsJsonAsync(urlApi, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var datos = JsonSerializer.Deserialize<UsuarioModel>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (datos != null)
                    {
                        HttpContext.Session.SetString("Autenticado", "1");
                        HttpContext.Session.SetString("Nombre", datos.Nombre ?? "Usuario");
                        HttpContext.Session.SetString("PrimerApellido", datos.PrimerApellido ?? "");
                        HttpContext.Session.SetInt32("IdUsuario", datos.IdUsuario);
                        HttpContext.Session.SetString("Token", datos.Token ?? "");
                        HttpContext.Session.SetInt32("IdRol", datos.IdRol);
                        HttpContext.Session.SetString("NombreRol", datos.NombreRol ?? "Usuario");
                        HttpContext.Session.SetString("TipoUsuario", datos.TipoUsuario ?? "Usuario");

                        return RedirectToAction("Dashboard", "Home");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Credenciales incorrectas o usuario inactivo";
                    return View(model);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Error al conectar con el servidor: " + error;
                    return View(model);
                }

                ViewBag.Error = "Error al iniciar sesión";
                return View(model);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error de conexión: " + ex.Message;
                return View(model);
            }
        }

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [SessionAuthorize]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GeneralDashboard()
        {
            return View();
        }

        public IActionResult LayoutDefault()
        {
            return View();
        }
    }
}