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
    public class AsistenciasController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public AsistenciasController(
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
                "Asistencias";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "No fue posible consultar las asistencias.";

                return View(new List<AsistenciaModel>());
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var asistencias =
                JsonSerializer.Deserialize<List<AsistenciaModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(
                asistencias ??
                new List<AsistenciaModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            await CargarMatriculas();

            return View(new AsistenciaModel
            {
                Fecha = DateTime.Now,
                Estado = "Presente"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            AsistenciaModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarMatriculas(model.IdMatricula);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Asistencias";

            var response =
                await client.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Asistencia registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible registrar la asistencia.");

            await CargarMatriculas(model.IdMatricula);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Asistencias/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var asistencia =
                JsonSerializer.Deserialize<AsistenciaModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(asistencia);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Asistencias/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var asistencia =
                JsonSerializer.Deserialize<AsistenciaModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (asistencia == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await CargarMatriculas(
                asistencia.IdMatricula);

            return View(asistencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            AsistenciaModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarMatriculas(model.IdMatricula);

                return View(model);
            }

            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                "Asistencias";

            var response =
                await client.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Asistencia actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible actualizar la asistencia.");

            await CargarMatriculas(model.IdMatricula);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Asistencias/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Asistencia eliminada correctamente.";
            }
            else
            {
                var contenido =
                    await response.Content.ReadAsStringAsync();

                TempData["Error"] = ObtenerMensajeError(
                    contenido,
                    "No fue posible eliminar la asistencia.");
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