using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Models;
using PracticaProgramada1.Filters;
using System.Net;
using System.Text.Json;
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
            try
            {
                using var client = _http.CreateClient();
                var urlApi = _config["Valores:UrlApi"] + "Login/login";

                var response = await client.PostAsJsonAsync(urlApi, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var datos = JsonSerializer.Deserialize<UsuarioModel>(
                        jsonResponse,
                        new JsonSerializerOptions
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


                        if (datos.IndicadorTemp)
                        {
                            HttpContext.Session.SetString("CambioObligatorio", "1");
                            return RedirectToAction("CambiarContrasena", "Home");
                        }

                        return RedirectToAction("Dashboard", "Home");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.Error = "Credenciales incorrectas o usuario inactivo";
                    return View(model);
                }
                else
                {
                    ViewBag.Error = "Error al conectar con el servidor";
                    return View(model);
                }

                ViewBag.Error = "Error al iniciar sesión";
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error de conexión";
                return View(model);
            }
        }

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            /*
             * Si todavía tiene pendiente el cambio de contraseña
             * obligatorio, lo mandamos de vuelta aunque intente
             * entrar directo por la URL del dashboard.
             */
            if (HttpContext.Session.GetString("CambioObligatorio") == "1")
                return RedirectToAction("CambiarContrasena", "Home");

            return View();
        }

        #region Cambiar Contraseña

        [SessionAuthorize]
        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            var model = new CambiarContrasenaModel
            {
                IdUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0
            };

            ViewBag.Obligatorio = HttpContext.Session.GetString("CambioObligatorio") == "1";

            return View(model);
        }

        [SessionAuthorize]
        [HttpPost]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaModel model)
        {
            model.IdUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            ViewBag.Obligatorio = HttpContext.Session.GetString("CambioObligatorio") == "1";

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var client = _http.CreateClient();
                var urlApi = _config["Valores:UrlApi"] + "Login/cambiar-contrasena";

                var response = await client.PostAsJsonAsync(urlApi, model);

                if (response.IsSuccessStatusCode)
                {
                    /*
                     * Ya estableció una contraseña definitiva:
                     * quitamos la bandera y lo dejamos pasar
                     * al dashboard.
                     */
                    HttpContext.Session.Remove("CambioObligatorio");
                    TempData["Success"] = "Contraseña actualizada correctamente.";
                    return RedirectToAction("Dashboard", "Home");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrió un error al conectar con el servidor.";
                return View(model);
            }
        }

        #endregion

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroEstudianteModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var client = _http.CreateClient();

                var urlApi = _config["Valores:UrlApi"] + "Login/registro-estudiante";

                var response = await client.PostAsJsonAsync(urlApi, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Usuario registrado correctamente.";

                    return RedirectToAction("Index");
                }

                var mensaje = await response.Content.ReadAsStringAsync();

                ViewBag.Error = mensaje;

                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrió un error al conectar con el servidor.";

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Recuperar(RecuperarModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var client = _http.CreateClient();
                var urlApi = _config["Valores:UrlApi"] + "Login/recuperar-acceso";

                var response = await client.PostAsJsonAsync(urlApi, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Se ha enviado una contraseña temporal a su correo.";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrió un error al conectar con el servidor.";
                return View(model);
            }
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