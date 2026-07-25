using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CalificacionesController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;

        public CalificacionesController(
            ICalificacionService calificacionService)
        {
            _calificacionService = calificacionService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var calificaciones = _calificacionService.Listar();

            return Ok(calificaciones);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la calificación no es válido."
                });
            }

            var calificacion = _calificacionService.Consultar(id);

            if (calificacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la calificación."
                });
            }

            return Ok(calificacion);
        }

        [HttpPost]
        public IActionResult Registrar(
            [FromBody] CalificacionModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la calificación."
                });
            }

            if (model.IdMatricula <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar una matrícula válida."
                });
            }

            if (model.Nota < 0 || model.Nota > 100)
            {
                return BadRequest(new
                {
                    mensaje = "La nota debe estar entre 0 y 100."
                });
            }

            var resultado = _calificacionService.Registrar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible registrar la calificación."
                });
            }

            return Ok(new
            {
                mensaje = "Calificación registrada correctamente."
            });
        }

        [HttpPut]
        public IActionResult Actualizar(
            [FromBody] CalificacionModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la calificación."
                });
            }

            if (model.IdCalificacion <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la calificación no es válido."
                });
            }

            if (model.IdMatricula <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar una matrícula válida."
                });
            }

            if (model.Nota < 0 || model.Nota > 100)
            {
                return BadRequest(new
                {
                    mensaje = "La nota debe estar entre 0 y 100."
                });
            }

            var calificacionExistente =
                _calificacionService.Consultar(model.IdCalificacion);

            if (calificacionExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la calificación que desea actualizar."
                });
            }

            var resultado = _calificacionService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar la calificación."
                });
            }

            return Ok(new
            {
                mensaje = "Calificación actualizada correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la calificación no es válido."
                });
            }

            var calificacion = _calificacionService.Consultar(id);

            if (calificacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la calificación que desea eliminar."
                });
            }

            var resultado = _calificacionService.Eliminar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible eliminar la calificación."
                });
            }

            return Ok(new
            {
                mensaje = "Calificación eliminada correctamente."
            });
        }
    }
}