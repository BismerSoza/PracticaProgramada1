using Microsoft.AspNetCore.Mvc;
using PracticaProgramada1API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PracticaProgramada1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] UsuarioModel model)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(); // útil para ver errores de conexión

            var usuario = await connection.QueryFirstOrDefaultAsync<UsuarioModel>(
                "spIniciarSesionUsuario",
                new { Correo = model.Correo, Contrasenna = model.Contrasenna },
                commandType: CommandType.StoredProcedure
            );

            if (usuario != null)
                return Ok(usuario);
            else
                return Unauthorized("Credenciales inválidas o usuario inactivo");
        }
    }
}
