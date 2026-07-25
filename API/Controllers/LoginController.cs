using API.Models;
using API.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@correo", model.Correo);

            var usuario = connection.QueryFirstOrDefault<DatosUsuarioResponseModel>(
                "spIniciarSesionUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (usuario != null && !string.IsNullOrEmpty(usuario.Contrasenna))
            {
                bool isValidBCryptFormat = usuario.Contrasenna.StartsWith("$2");

                if (!isValidBCryptFormat)
                {
                    return StatusCode(500, "Error de configuración de seguridad. Las credenciales deben ser actualizadas por un administrador.");
                }

                try
                {
                    if (BCrypt.Net.BCrypt.Verify(model.Contrasenna, usuario.Contrasenna))
                    {
                        usuario.Token = _utiles.GenerarToken(usuario.IdUsuario);

                        usuario.Contrasenna = string.Empty;
                        return Ok(usuario);
                    }
                }
                catch (BCrypt.Net.SaltParseException)
                {

                    return StatusCode(500, "Error al procesar las credenciales. Contacte al administrador.");
                }
            }

            return Unauthorized("Credenciales inválidas o usuario inactivo");
        }
    }
}