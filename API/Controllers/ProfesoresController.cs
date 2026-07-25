using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesoresController : ControllerBase
    {
        private readonly IProfesorService _profesorService;

        public ProfesoresController(IProfesorService profesorService)
        {
            _profesorService = profesorService;
        }

        [HttpGet("listar")]
        public IActionResult Listar()
        {
            var datos = _profesorService.Listar();

            return Ok(datos);
        }

        [HttpGet("consultar/{id}")]
        public IActionResult Consultar(int id)
        {
            var profesor = _profesorService.Consultar(id);

            if (profesor == null)
            {
                return NotFound(new
                {
                    Exitoso = false,
                    Mensaje = "No se encontró el profesor."
                });
            }

            return Ok(profesor);
        }

        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] ProfesorModel model)
        {
            var resultado = _profesorService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    Exitoso = false,
                    Mensaje = "No fue posible actualizar el profesor."
                });
            }

            return Ok(new
            {
                Exitoso = true,
                Mensaje = "Profesor actualizado correctamente."
            });
        }

        [HttpDelete("desactivar/{id}")]
        public IActionResult Desactivar(int id)
        {
            var resultado = _profesorService.Desactivar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    Exitoso = false,
                    Mensaje = "No fue posible desactivar el profesor."
                });
            }

            return Ok(new
            {
                Exitoso = true,
                Mensaje = "Profesor desactivado correctamente."
            });
        }
    }
}