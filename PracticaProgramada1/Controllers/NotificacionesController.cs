using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Filters;
using PracticaProgramada1.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PracticaProgramada1.Controllers
{
    [SessionAuthorize]
    public class NotificacionesController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public NotificacionesController(
            IHttpClientFactory http,
            IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private HttpClient CrearCliente()
        {
            var client = _http.CreateClient();

            var token =
                HttpContext.Session.GetString("Token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Notificaciones";

            var response =
                await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return View(
                    new List<NotificacionModel>());
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var notificaciones =
                JsonSerializer.Deserialize<
                    List<NotificacionModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(
                notificaciones ??
                new List<NotificacionModel>());
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new NotificacionModel
            {
                FechaEnvio = DateTime.Now,
                Leida = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(
            NotificacionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Notificaciones";

            var response =
                await client.PostAsJsonAsync(
                    url,
                    model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.Error =
                "No fue posible registrar la notificación.";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Notificaciones/{id}";

            var response =
                await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var notificacion =
                JsonSerializer.Deserialize<
                    NotificacionModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(notificacion);
        }

        [HttpGet]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Notificaciones/{id}";

            var response =
                await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var notificacion =
                JsonSerializer.Deserialize<
                    NotificacionModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (notificacion == null)
            {
                return RedirectToAction(
                    nameof(Index));
            }

            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.Now;

            var actualizarUrl =
                _config["Valores:UrlApi"] +
                "Notificaciones";

            await client.PutAsJsonAsync(
                actualizarUrl,
                notificacion);

            return RedirectToAction(
                nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Notificaciones/{id}";

            await client.DeleteAsync(url);

            return RedirectToAction(
                nameof(Index));
        }
    }
}