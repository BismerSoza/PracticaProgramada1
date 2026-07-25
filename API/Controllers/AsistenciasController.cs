using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AsistenciasController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciasController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var asistencias = _asistenciaService.Listar();

            return Ok(asistencias);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la asistencia no es válido."
                });
            }

            var asistencia = _asistenciaService.Consultar(id);

            if (asistencia == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la asistencia."
                });
            }

            return Ok(asistencia);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] AsistenciaModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la asistencia."
                });
            }

            if (model.IdMatricula <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar una matrícula válida."
                });
            }

            if (model.Fecha == default)
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar la fecha de la asistencia."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Estado))
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar el estado de la asistencia."
                });
            }

            var estado = model.Estado.Trim().ToLower();

            if (estado != "presente" &&
                estado != "ausente" &&
                estado != "justificada")
            {
                return BadRequest(new
                {
                    mensaje = "El estado debe ser Presente, Ausente o Justificada."
                });
            }

            var resultado = _asistenciaService.Registrar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible registrar la asistencia."
                });
            }

            return Ok(new
            {
                mensaje = "Asistencia registrada correctamente."
            });
        }

        [HttpPut]
        public IActionResult Actualizar([FromBody] AsistenciaModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la asistencia."
                });
            }

            if (model.IdAsistencia <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la asistencia no es válido."
                });
            }

            if (model.IdMatricula <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar una matrícula válida."
                });
            }

            if (model.Fecha == default)
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar la fecha de la asistencia."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Estado))
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar el estado de la asistencia."
                });
            }

            var estado = model.Estado.Trim().ToLower();

            if (estado != "presente" &&
                estado != "ausente" &&
                estado != "justificada")
            {
                return BadRequest(new
                {
                    mensaje = "El estado debe ser Presente, Ausente o Justificada."
                });
            }

            var asistenciaExistente =
                _asistenciaService.Consultar(model.IdAsistencia);

            if (asistenciaExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la asistencia que desea actualizar."
                });
            }

            var resultado = _asistenciaService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar la asistencia."
                });
            }

            return Ok(new
            {
                mensaje = "Asistencia actualizada correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la asistencia no es válido."
                });
            }

            var asistencia = _asistenciaService.Consultar(id);

            if (asistencia == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la asistencia que desea eliminar."
                });
            }

            var resultado = _asistenciaService.Eliminar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible eliminar la asistencia."
                });
            }

            return Ok(new
            {
                mensaje = "Asistencia eliminada correctamente."
            });
        }
    }
}