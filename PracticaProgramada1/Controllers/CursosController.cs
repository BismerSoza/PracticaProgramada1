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
    public class CursosController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public CursosController(
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

        private async Task CargarProfesores(int? idProfesorSeleccionado = null)
        {
            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] +
                      "Profesores/listar";

            var response = await client.GetAsync(url);

            var profesores = new List<ProfesorModel>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                profesores =
                    JsonSerializer.Deserialize<List<ProfesorModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<ProfesorModel>();
            }

            var profesoresActivos = profesores
                .Where(x => x.Estado)
                .Select(x => new
                {
                    x.IdProfesor,
                    NombreCompleto =
                        $"{x.Nombre} {x.PrimerApellido} {x.SegundoApellido}"
                            .Replace("  ", " ")
                            .Trim()
                })
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            ViewBag.Profesores = new SelectList(
                profesoresActivos,
                "IdProfesor",
                "NombreCompleto",
                idProfesorSeleccionado);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] + "Cursos";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "No fue posible consultar los cursos.";

                return View(new List<CursoModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var cursos =
                JsonSerializer.Deserialize<List<CursoModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(cursos ?? new List<CursoModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            await CargarProfesores();

            return View(new CursoModel
            {
                Estado = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(CursoModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarProfesores(model.IdProfesor);

                return View(model);
            }

            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] + "Cursos";

            var response = await client.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Curso registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible registrar el curso.");

            await CargarProfesores(model.IdProfesor);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Cursos/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var curso =
                JsonSerializer.Deserialize<CursoModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(curso);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Cursos/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var curso =
                JsonSerializer.Deserialize<CursoModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (curso == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await CargarProfesores(curso.IdProfesor);

            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CursoModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarProfesores(model.IdProfesor);

                return View(model);
            }

            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] + "Cursos";

            var response = await client.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Curso actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible actualizar el curso.");

            await CargarProfesores(model.IdProfesor);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Cursos/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Curso desactivado correctamente.";
            }
            else
            {
                TempData["Error"] =
                    "No fue posible desactivar el curso.";
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