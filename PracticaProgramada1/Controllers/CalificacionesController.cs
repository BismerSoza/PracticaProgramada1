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
    public class CalificacionesController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public CalificacionesController(
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

        private async Task CargarMatriculas(
            int? idMatriculaSeleccionada = null)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Matriculas";

            var response = await client.GetAsync(url);

            var matriculas = new List<MatriculaModel>();

            if (response.IsSuccessStatusCode)
            {
                var json =
                    await response.Content.ReadAsStringAsync();

                matriculas =
                    JsonSerializer.Deserialize<List<MatriculaModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<MatriculaModel>();
            }

            var matriculasActivas = matriculas
                .Where(x => x.Estado)
                .Select(x => new
                {
                    x.IdMatricula,

                    Descripcion =
                        $"{x.Estudiante ?? $"Estudiante #{x.IdEstudiante}"} - " +
                        $"{x.NombreCurso ?? $"Curso #{x.IdCurso}"}"
                })
                .OrderBy(x => x.Descripcion)
                .ToList();

            ViewBag.Matriculas = new SelectList(
                matriculasActivas,
                "IdMatricula",
                "Descripcion",
                idMatriculaSeleccionada);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Calificaciones";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "No fue posible consultar las calificaciones.";

                return View(new List<CalificacionModel>());
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var calificaciones =
                JsonSerializer.Deserialize<List<CalificacionModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(
                calificaciones ??
                new List<CalificacionModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            await CargarMatriculas();

            return View(new CalificacionModel
            {
                FechaRegistro = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            CalificacionModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarMatriculas(model.IdMatricula);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Calificaciones";

            var response =
                await client.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Calificación registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible registrar la calificación.");

            await CargarMatriculas(model.IdMatricula);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Calificaciones/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var calificacion =
                JsonSerializer.Deserialize<CalificacionModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(calificacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Calificaciones/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var calificacion =
                JsonSerializer.Deserialize<CalificacionModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (calificacion == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await CargarMatriculas(
                calificacion.IdMatricula);

            return View(calificacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            CalificacionModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarMatriculas(model.IdMatricula);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Calificaciones";

            var response =
                await client.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Calificación actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible actualizar la calificación.");

            await CargarMatriculas(model.IdMatricula);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Calificaciones/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Calificación eliminada correctamente.";
            }
            else
            {
                var contenido =
                    await response.Content.ReadAsStringAsync();

                TempData["Error"] = ObtenerMensajeError(
                    contenido,
                    "No fue posible eliminar la calificación.");
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