using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController
        : ControllerBase
    {
        private readonly IEstudianteService
            _estudianteService;

        public EstudiantesController(
            IEstudianteService estudianteService)
        {
            _estudianteService =
                estudianteService;
        }

        [HttpGet("listar")]
        public IActionResult Listar()
        {
            try
            {
                var estudiantes =
                    _estudianteService.Listar();

                return Ok(estudiantes);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible listar los estudiantes.",
                        Detalle = ex.Message
                    });
            }
        }

        [HttpGet("consultar/{id:int}")]
        public IActionResult Consultar(int id)
        {
            try
            {
                var estudiante =
                    _estudianteService
                        .Consultar(id);

                if (estudiante == null)
                {
                    return NotFound(new
                    {
                        Exitoso = false,
                        Mensaje =
                            "El estudiante no existe."
                    });
                }

                return Ok(estudiante);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible consultar el estudiante.",
                        Detalle = ex.Message
                    });
            }
        }

        [HttpPut("actualizar")]
        public IActionResult Actualizar(
            [FromBody] EstudianteModel model)
        {
            if (model.IdEstudiante <= 0)
            {
                return BadRequest(new
                {
                    Exitoso = false,
                    Mensaje =
                        "El identificador del estudiante no es válido."
                });
            }

            try
            {
                var resultado =
                    _estudianteService
                        .Actualizar(model);

                if (!resultado)
                {
                    return BadRequest(new
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible actualizar el estudiante."
                    });
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje =
                        "Estudiante actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ocurrió un error al actualizar el estudiante.",
                        Detalle = ex.Message
                    });
            }
        }

        [HttpDelete("desactivar/{id:int}")]
        public IActionResult Desactivar(int id)
        {
            try
            {
                var resultado =
                    _estudianteService
                        .Desactivar(id);

                if (!resultado)
                {
                    return BadRequest(new
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible desactivar el estudiante."
                    });
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje =
                        "Estudiante desactivado correctamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ocurrió un error al desactivar el estudiante.",
                        Detalle = ex.Message
                    });
            }
        }

        [HttpPut("activar/{id:int}")]
        public IActionResult Activar(int id)
        {
            try
            {
                var resultado =
                    _estudianteService
                        .Activar(id);

                if (!resultado)
                {
                    return BadRequest(new
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible activar el estudiante."
                    });
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje =
                        "Estudiante activado correctamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ocurrió un error al activar el estudiante.",
                        Detalle = ex.Message
                    });
            }
        }
    }
}