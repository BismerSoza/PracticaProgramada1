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
    public class MatriculasController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public MatriculasController(
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

        private async Task CargarListas(
            int? idEstudianteSeleccionado = null,
            int? idCursoSeleccionado = null)
        {
            using var client = CrearCliente();

            var urlBase = _config["Valores:UrlApi"];

            var estudiantes = new List<EstudianteModel>();
            var cursos = new List<CursoModel>();

            var respuestaEstudiantes =
                await client.GetAsync(
                    urlBase + "Estudiantes/listar");

            if (respuestaEstudiantes.IsSuccessStatusCode)
            {
                var jsonEstudiantes =
                    await respuestaEstudiantes.Content
                        .ReadAsStringAsync();

                estudiantes =
                    JsonSerializer.Deserialize<List<EstudianteModel>>(
                        jsonEstudiantes,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<EstudianteModel>();
            }

            var respuestaCursos =
                await client.GetAsync(urlBase + "Cursos");

            if (respuestaCursos.IsSuccessStatusCode)
            {
                var jsonCursos =
                    await respuestaCursos.Content.ReadAsStringAsync();

                cursos =
                    JsonSerializer.Deserialize<List<CursoModel>>(
                        jsonCursos,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<CursoModel>();
            }

            var estudiantesActivos = estudiantes
                .Where(x => x.Estado)
                .Select(x => new
                {
                    x.IdEstudiante,

                    NombreCompleto =
                        $"{x.Nombre} {x.PrimerApellido} {x.SegundoApellido}"
                            .Replace("  ", " ")
                            .Trim()
                })
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            var cursosActivos = cursos
                .Where(x => x.Estado)
                .OrderBy(x => x.NombreCurso)
                .ToList();

            ViewBag.Estudiantes = new SelectList(
                estudiantesActivos,
                "IdEstudiante",
                "NombreCompleto",
                idEstudianteSeleccionado);

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

            var url = _config["Valores:UrlApi"] + "Matriculas";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "No fue posible consultar las matrículas.";

                return View(new List<MatriculaModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var matriculas =
                JsonSerializer.Deserialize<List<MatriculaModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(matriculas ?? new List<MatriculaModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            await CargarListas();

            return View(new MatriculaModel
            {
                FechaMatricula = DateTime.Now,
                Estado = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            MatriculaModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas(
                    model.IdEstudiante,
                    model.IdCurso);

                return View(model);
            }

            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] + "Matriculas";

            var response = await client.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Matrícula registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible registrar la matrícula.");

            await CargarListas(
                model.IdEstudiante,
                model.IdCurso);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Matriculas/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var matricula =
                JsonSerializer.Deserialize<MatriculaModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(matricula);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Matriculas/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var matricula =
                JsonSerializer.Deserialize<MatriculaModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (matricula == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await CargarListas(
                matricula.IdEstudiante,
                matricula.IdCurso);

            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            MatriculaModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas(
                    model.IdEstudiante,
                    model.IdCurso);

                return View(model);
            }

            using var client = CrearCliente();

            var url = _config["Valores:UrlApi"] + "Matriculas";

            var response = await client.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Matrícula actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            var contenido =
                await response.Content.ReadAsStringAsync();

            ViewBag.Error = ObtenerMensajeError(
                contenido,
                "No fue posible actualizar la matrícula.");

            await CargarListas(
                model.IdEstudiante,
                model.IdCurso);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            using var client = CrearCliente();

            var url =
                _config["Valores:UrlApi"] +
                $"Matriculas/{id}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] =
                    "Matrícula desactivada correctamente.";
            }
            else
            {
                TempData["Error"] =
                    "No fue posible desactivar la matrícula.";
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