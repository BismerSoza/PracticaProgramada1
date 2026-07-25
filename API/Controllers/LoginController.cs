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

            if (usuario != null && BCrypt.Net.BCrypt.Verify(model.Contrasenna, usuario.Contrasenna))
            {
                usuario.Token = _utiles.GenerarToken(usuario.IdUsuario);
                return Ok(usuario);
            }

            return Unauthorized("Credenciales inválidas o usuario inactivo");
        }
    }
}