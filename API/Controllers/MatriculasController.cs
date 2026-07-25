using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatriculasController : ControllerBase
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculasController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var matriculas = _matriculaService.Listar();

            return Ok(matriculas);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la matrícula no es válido."
                });
            }

            var matricula = _matriculaService.Consultar(id);

            if (matricula == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la matrícula."
                });
            }

            return Ok(matricula);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] MatriculaModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la matrícula."
                });
            }

            if (model.IdEstudiante <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un estudiante válido."
                });
            }

            if (model.IdCurso <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un curso válido."
                });
            }

            var resultado = _matriculaService.Registrar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible registrar la matrícula."
                });
            }

            return Ok(new
            {
                mensaje = "Matrícula registrada correctamente."
            });
        }

        [HttpPut]
        public IActionResult Actualizar([FromBody] MatriculaModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la matrícula."
                });
            }

            if (model.IdMatricula <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la matrícula no es válido."
                });
            }

            if (model.IdEstudiante <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un estudiante válido."
                });
            }

            if (model.IdCurso <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un curso válido."
                });
            }

            var matriculaExistente =
                _matriculaService.Consultar(model.IdMatricula);

            if (matriculaExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la matrícula que desea actualizar."
                });
            }

            var resultado = _matriculaService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar la matrícula."
                });
            }

            return Ok(new
            {
                mensaje = "Matrícula actualizada correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Desactivar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la matrícula no es válido."
                });
            }

            var matricula = _matriculaService.Consultar(id);

            if (matricula == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la matrícula que desea desactivar."
                });
            }

            var resultado = _matriculaService.Desactivar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible desactivar la matrícula."
                });
            }

            return Ok(new
            {
                mensaje = "Matrícula desactivada correctamente."
            });
        }
    }
}