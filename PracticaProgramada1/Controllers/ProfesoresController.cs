using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1.Filters;
using PracticaProgramada1.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PracticaProgramada1.Controllers
{
    [SessionAuthorize]
    public class ProfesoresController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public ProfesoresController(IHttpClientFactory http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + "Profesores/listar";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View(new List<ProfesorModel>());

            var json = await response.Content.ReadAsStringAsync();

            var profesores = JsonSerializer.Deserialize<List<ProfesorModel>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(profesores);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            using var client = _http.CreateClient();

            var response = await client.GetAsync(
                _config["Valores:UrlApi"] + $"Profesores/consultar/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();

            var profesor = JsonSerializer.Deserialize<ProfesorModel>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(profesor);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using var client = _http.CreateClient();

            var response = await client.GetAsync(
                _config["Valores:UrlApi"] + $"Profesores/consultar/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();

            var profesor = JsonSerializer.Deserialize<ProfesorModel>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(profesor);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ProfesorModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = _http.CreateClient();

            var response = await client.PutAsJsonAsync(
                _config["Valores:UrlApi"] + "Profesores/actualizar",
                model);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "No fue posible actualizar el profesor.";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            using var client = _http.CreateClient();

            await client.DeleteAsync(
                _config["Valores:UrlApi"] + $"Profesores/desactivar/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}