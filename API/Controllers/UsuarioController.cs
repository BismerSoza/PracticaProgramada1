using API.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] RegistroUsuarioModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string contrasennaHasheada = BCrypt.Net.BCrypt.HashPassword(model.Contrasenna);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@correo", model.Correo);
            parameters.Add("@contrasenna", contrasennaHasheada);
            parameters.Add("@nombre", model.Nombre);
            parameters.Add("@primerApellido", model.PrimerApellido);
            parameters.Add("@identificacion", model.Identificacion);
            parameters.Add("@idRol", model.IdRol);

            try
            {
                var resultado = connection.QueryFirstOrDefault<int>(
                    "spRegistrarUsuario", // Debes crear este stored procedure
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (resultado > 0)
                {
                    return Created("", new { mensaje = "Usuario registrado exitosamente", idUsuario = resultado });
                }
                else
                {
                    return BadRequest(new { error = "No se pudo registrar el usuario. El correo puede estar en uso." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al registrar el usuario" });
            }
        }

        [HttpPost("actualizar-contrasenna")]
        public IActionResult ActualizarContrasenna([FromBody] ActualizarContrasennaModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string contrasennaHasheada = BCrypt.Net.BCrypt.HashPassword(model.ContrasennaActual);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@idUsuario", model.IdUsuario);
            parameters.Add("@contrasennaHasheada", contrasennaHasheada);

            try
            {
                connection.Execute(
                    "spActualizarContrasenna",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new { mensaje = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al actualizar la contraseña" });
            }
        }
    }
}
