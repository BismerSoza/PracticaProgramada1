using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CursosController : ControllerBase
    {
        private readonly ICursoService _cursoService;

        public CursosController(ICursoService cursoService)
        {
            _cursoService = cursoService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var cursos = _cursoService.Listar();

            return Ok(cursos);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del curso no es válido."
                });
            }

            var curso = _cursoService.Consultar(id);

            if (curso == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el curso."
                });
            }

            return Ok(curso);
        }

        [HttpPut]
        public IActionResult Actualizar([FromBody] CursoModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos del curso."
                });
            }

            if (model.IdCurso <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del curso no es válido."
                });
            }

            if (model.IdProfesor <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un profesor válido."
                });
            }

            if (string.IsNullOrWhiteSpace(model.NombreCurso))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre del curso es obligatorio."
                });
            }

            var cursoExistente = _cursoService.Consultar(model.IdCurso);

            if (cursoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el curso que desea actualizar."
                });
            }

            var resultado = _cursoService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar el curso."
                });
            }

            return Ok(new
            {
                mensaje = "Curso actualizado correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Desactivar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del curso no es válido."
                });
            }

            var curso = _cursoService.Consultar(id);

            if (curso == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el curso que desea desactivar."
                });
            }

            var resultado = _cursoService.Desactivar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible desactivar el curso."
                });
            }

            return Ok(new
            {
                mensaje = "Curso desactivado correctamente."
            });
        }
    }
}