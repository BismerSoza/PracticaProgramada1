using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;

        public EventosController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var eventos = _eventoService.Listar();

            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del evento no es válido."
                });
            }

            var evento = _eventoService.Consultar(id);

            if (evento == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el evento."
                });
            }

            return Ok(evento);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] EventoModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos del evento."
                });
            }

            if (model.IdCurso <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un curso válido."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Titulo))
            {
                return BadRequest(new
                {
                    mensaje = "El título del evento es obligatorio."
                });
            }

            if (model.FechaEvento == default)
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar la fecha del evento."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Lugar))
            {
                return BadRequest(new
                {
                    mensaje = "El lugar del evento es obligatorio."
                });
            }

            var resultado = _eventoService.Registrar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible registrar el evento."
                });
            }

            return Ok(new
            {
                mensaje = "Evento registrado correctamente."
            });
        }

        [HttpPut]
        public IActionResult Actualizar([FromBody] EventoModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos del evento."
                });
            }

            if (model.IdEvento <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del evento no es válido."
                });
            }

            if (model.IdCurso <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un curso válido."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Titulo))
            {
                return BadRequest(new
                {
                    mensaje = "El título del evento es obligatorio."
                });
            }

            if (model.FechaEvento == default)
            {
                return BadRequest(new
                {
                    mensaje = "Debe indicar la fecha del evento."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Lugar))
            {
                return BadRequest(new
                {
                    mensaje = "El lugar del evento es obligatorio."
                });
            }

            var eventoExistente = _eventoService.Consultar(model.IdEvento);

            if (eventoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el evento que desea actualizar."
                });
            }

            var resultado = _eventoService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar el evento."
                });
            }

            return Ok(new
            {
                mensaje = "Evento actualizado correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Desactivar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador del evento no es válido."
                });
            }

            var evento = _eventoService.Consultar(id);

            if (evento == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el evento que desea desactivar."
                });
            }

            var resultado = _eventoService.Desactivar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible desactivar el evento."
                });
            }

            return Ok(new
            {
                mensaje = "Evento desactivado correctamente."
            });
        }
    }
}