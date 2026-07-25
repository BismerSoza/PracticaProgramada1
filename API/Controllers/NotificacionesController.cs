using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(
            INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var notificaciones = _notificacionService.Listar();

            return Ok(notificaciones);
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la notificación no es válido."
                });
            }

            var notificacion = _notificacionService.Consultar(id);

            if (notificacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la notificación."
                });
            }

            return Ok(notificacion);
        }

        [HttpPost]
        public IActionResult Registrar(
            [FromBody] NotificacionModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la notificación."
                });
            }

            if (model.IdUsuario <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Asunto))
            {
                return BadRequest(new
                {
                    mensaje = "El asunto de la notificación es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Mensaje))
            {
                return BadRequest(new
                {
                    mensaje = "El mensaje de la notificación es obligatorio."
                });
            }

            var resultado = _notificacionService.Registrar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible registrar la notificación."
                });
            }

            return Ok(new
            {
                mensaje = "Notificación registrada correctamente."
            });
        }

        [HttpPut]
        public IActionResult Actualizar(
            [FromBody] NotificacionModel model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    mensaje = "Debe proporcionar los datos de la notificación."
                });
            }

            if (model.IdNotificacion <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la notificación no es válido."
                });
            }

            if (model.IdUsuario <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe seleccionar un usuario válido."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Asunto))
            {
                return BadRequest(new
                {
                    mensaje = "El asunto de la notificación es obligatorio."
                });
            }

            if (string.IsNullOrWhiteSpace(model.Mensaje))
            {
                return BadRequest(new
                {
                    mensaje = "El mensaje de la notificación es obligatorio."
                });
            }

            var notificacionExistente =
                _notificacionService.Consultar(model.IdNotificacion);

            if (notificacionExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la notificación que desea actualizar."
                });
            }

            var resultado = _notificacionService.Actualizar(model);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible actualizar la notificación."
                });
            }

            return Ok(new
            {
                mensaje = "Notificación actualizada correctamente."
            });
        }

        [HttpPatch("{id}/leer")]
        public IActionResult MarcarComoLeida(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la notificación no es válido."
                });
            }

            var notificacion = _notificacionService.Consultar(id);

            if (notificacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la notificación."
                });
            }

            if (notificacion.Leida)
            {
                return Ok(new
                {
                    mensaje = "La notificación ya estaba marcada como leída."
                });
            }

            var resultado = _notificacionService.MarcarComoLeida(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible marcar la notificación como leída."
                });
            }

            return Ok(new
            {
                mensaje = "Notificación marcada como leída correctamente."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El identificador de la notificación no es válido."
                });
            }

            var notificacion = _notificacionService.Consultar(id);

            if (notificacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la notificación que desea eliminar."
                });
            }

            var resultado = _notificacionService.Eliminar(id);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No fue posible eliminar la notificación."
                });
            }

            return Ok(new
            {
                mensaje = "Notificación eliminada correctamente."
            });
        }
    }
}