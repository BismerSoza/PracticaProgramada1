using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PracticaProgramada1.Filters;
using PracticaProgramada1.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PracticaProgramada1.Controllers
{
    [SessionAuthorize]
    public class EventosController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public EventosController(
            IHttpClientFactory http,
            IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private HttpClient CrearCliente()
        {
            var client = _http.CreateClient();

            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private async Task CargarCursos(
            int? idCursoSeleccionado = null)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Cursos";

            var response = await client.GetAsync(url);

            var cursos = new List<CursoModel>();

            if (response.IsSuccessStatusCode)
            {
                var json =
                    await response.Content.ReadAsStringAsync();

                cursos =
                    JsonSerializer.Deserialize<List<CursoModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<CursoModel>();
            }

            var cursosActivos = cursos
                .Where(x => x.Estado)
                .OrderBy(x => x.NombreCurso)
                .ToList();

            ViewBag.Cursos = new SelectList(
                cursosActivos,
                "IdCurso",
                "NombreCurso",
                idCursoSeleccionado);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Eventos";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "No fue posible consultar los eventos.";

                return View(new List<EventoModel>());
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var eventos =
                JsonSerializer.Deserialize<List<EventoModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(eventos ?? new List<EventoModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            await CargarCursos();

            return View(new EventoModel
            {
                FechaEvento = DateTime.Now,
                Estado = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            EventoModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCursos(model.IdCurso);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Eventos";

            var response =
                await client.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Evento registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible registrar el evento.");

            await CargarCursos(model.IdCurso);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Eventos/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var evento =
                JsonSerializer.Deserialize<EventoModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(evento);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Eventos/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var evento =
                JsonSerializer.Deserialize<EventoModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (evento == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await CargarCursos(evento.IdCurso);

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            EventoModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCursos(model.IdCurso);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Eventos";

            var response =
                await client.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Evento actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible actualizar el evento.");

            await CargarCursos(model.IdCurso);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Eventos/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Evento desactivado correctamente.";
            }
            else
            {
                var contenido =
                    await response.Content.ReadAsStringAsync();

                TempData["Error"] = ObtenerMensajeError(
                    contenido,
                    "No fue posible desactivar el evento.");
            }

            return RedirectToAction(nameof(Index));
        }

        private static string ObtenerMensajeError(
            string contenido,
            string mensajePredeterminado)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return mensajePredeterminado;
            }

            try
            {
                using var documento =
                    JsonDocument.Parse(contenido);

                if (documento.RootElement.TryGetProperty(
                    "mensaje",
                    out var mensaje))
                {
                    return mensaje.GetString()
                           ?? mensajePredeterminado;
                }

                if (documento.RootElement.TryGetProperty(
                    "Mensaje",
                    out var mensajeMayuscula))
                {
                    return mensajeMayuscula.GetString()
                           ?? mensajePredeterminado;
                }
            }
            catch (JsonException)
            {
                return mensajePredeterminado;
            }

            return mensajePredeterminado;
        }
    }
}