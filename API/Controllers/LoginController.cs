using Microsoft.AspNetCore.Mvc;
using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUtilesService _utiles;

        public LoginController(IConfiguration config, IUtilesService utiles)
        {
            _config = config;
            _utiles = utiles;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] InicioSesionRequestModel model)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@correo", model.Correo);

                var usuario = connection.QueryFirstOrDefault<DatosUsuarioResponseModel>(
                    "spIniciarSesionUsuario",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (usuario == null)
                {
                    return Unauthorized("Usuario no encontrado o inactivo");
                }

                bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Contrasenna, usuario.Contrasenna);

                if (passwordValid)
                {
                    usuario.Token = _utiles.GenerarToken(usuario.IdUsuario);
                    return Ok(usuario);
                }

                return Unauthorized("Contraseña incorrecta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}