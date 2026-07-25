using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Filters;
using PracticaProgramada1.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PracticaProgramada1.Controllers
{
    [SessionAuthorize]
    public class EstudiantesController
        : Controller
    {
        private readonly IHttpClientFactory
            _http;

        private readonly IConfiguration
            _config;

        private readonly JsonSerializerOptions
            _jsonOptions;

        public EstudiantesController(
            IHttpClientFactory http,
            IConfiguration config)
        {
            _http = http;
            _config = config;

            _jsonOptions =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive =
                        true
                };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                "Estudiantes/listar";

            try
            {
                var response =
                    await client.GetAsync(url);

                if (!response
                    .IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        await ObtenerMensajeError(
                            response,
                            "No fue posible consultar los estudiantes.");

                    return View(
                        new List<EstudianteModel>());
                }

                var json =
                    await response.Content
                        .ReadAsStringAsync();

                var estudiantes =
                    JsonSerializer.Deserialize<
                        List<EstudianteModel>>(
                        json,
                        _jsonOptions);

                return View(
                    estudiantes ??
                    new List<EstudianteModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "No fue posible conectar con la API. " +
                    ex.Message;

                return View(
                    new List<EstudianteModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            return View(
                new RegistroEstudianteModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            RegistroEstudianteModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                "Login/registro-estudiante";

            try
            {
                var response =
                    await client.PostAsJsonAsync(
                        url,
                        model);

                if (response
                    .IsSuccessStatusCode)
                {
                    TempData["Mensaje"] =
                        "Estudiante registrado correctamente.";

                    return RedirectToAction(
                        nameof(Index));
                }

                ViewBag.Error =
                    await ObtenerMensajeError(
                        response,
                        "No fue posible registrar el estudiante.");

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "No fue posible conectar con la API. " +
                    ex.Message;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(
            int id)
        {
            var estudiante =
                await ConsultarEstudiante(id);

            if (estudiante == null)
            {
                TempData["Error"] =
                    "No fue posible consultar el estudiante.";

                return RedirectToAction(
                    nameof(Index));
            }

            return View(estudiante);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(
            int id)
        {
            var estudiante =
                await ConsultarEstudiante(id);

            if (estudiante == null)
            {
                TempData["Error"] =
                    "No fue posible consultar el estudiante.";

                return RedirectToAction(
                    nameof(Index));
            }

            return View(estudiante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            EstudianteModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                "Estudiantes/actualizar";

            try
            {
                var response =
                    await client.PutAsJsonAsync(
                        url,
                        model);

                if (response
                    .IsSuccessStatusCode)
                {
                    TempData["Mensaje"] =
                        "Estudiante actualizado correctamente.";

                    return RedirectToAction(
                        nameof(Index));
                }

                ViewBag.Error =
                    await ObtenerMensajeError(
                        response,
                        "No fue posible actualizar el estudiante.");

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "No fue posible conectar con la API. " +
                    ex.Message;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(
            int id)
        {
            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                $"Estudiantes/desactivar/{id}";

            try
            {
                var response =
                    await client.DeleteAsync(url);

                if (response
                    .IsSuccessStatusCode)
                {
                    TempData["Mensaje"] =
                        "Estudiante desactivado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        await ObtenerMensajeError(
                            response,
                            "No fue posible desactivar el estudiante.");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "No fue posible conectar con la API. " +
                    ex.Message;
            }

            return RedirectToAction(
                nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Activar(
            int id)
        {
            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                $"Estudiantes/activar/{id}";

            try
            {
                var response =
                    await client.PutAsync(
                        url,
                        null);

                if (response
                    .IsSuccessStatusCode)
                {
                    TempData["Mensaje"] =
                        "Estudiante activado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        await ObtenerMensajeError(
                            response,
                            "No fue posible activar el estudiante.");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "No fue posible conectar con la API. " +
                    ex.Message;
            }

            return RedirectToAction(
                nameof(Index));
        }

        private async Task<EstudianteModel?>
            ConsultarEstudiante(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            using var client =
                _http.CreateClient();

            var url =
                _config["Valores:UrlApi"] +
                $"Estudiantes/consultar/{id}";

            try
            {
                var response =
                    await client.GetAsync(url);

                if (!response
                    .IsSuccessStatusCode)
                {
                    return null;
                }

                var json =
                    await response.Content
                        .ReadAsStringAsync();

                return JsonSerializer
                    .Deserialize<EstudianteModel>(
                        json,
                        _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string>
            ObtenerMensajeError(
                HttpResponseMessage response,
                string mensajePredeterminado)
        {
            var contenido =
                await response.Content
                    .ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(
                contenido))
            {
                return mensajePredeterminado;
            }

            try
            {
                using var documento =
                    JsonDocument.Parse(contenido);

                var raiz =
                    documento.RootElement;

                if (raiz.TryGetProperty(
                    "Mensaje",
                    out var mensajeMayuscula))
                {
                    return mensajeMayuscula
                               .GetString()
                           ?? mensajePredeterminado;
                }

                if (raiz.TryGetProperty(
                    "mensaje",
                    out var mensajeMinuscula))
                {
                    return mensajeMinuscula
                               .GetString()
                           ?? mensajePredeterminado;
                }

                if (raiz.TryGetProperty(
                    "title",
                    out var titulo))
                {
                    return titulo.GetString()
                           ?? mensajePredeterminado;
                }
            }
            catch (JsonException)
            {
                return contenido;
            }

            return mensajePredeterminado;
        }
    }
}