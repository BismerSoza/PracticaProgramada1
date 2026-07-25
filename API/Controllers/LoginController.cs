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

        public LoginController(
            IConfiguration config,
            IUtilesService utiles)
        {
            _config = config;
            _utiles = utiles;
        }

        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] InicioSesionRequestModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@correo", model.Correo);

            var usuario =
                connection.QueryFirstOrDefault<DatosUsuarioResponseModel>(
                    "spIniciarSesionUsuario",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

            if (usuario != null &&
                BCrypt.Net.BCrypt.Verify(
                    model.Contrasenna,
                    usuario.Contrasenna))
            {
                usuario.Token =
                    _utiles.GenerarToken(usuario.IdUsuario);

                return Ok(usuario);
            }

            return Unauthorized(
                "Credenciales inválidas o usuario inactivo");
        }

        [HttpPost("registro-estudiante")]
        public IActionResult RegistrarEstudiante(
            [FromBody] RegistroEstudianteRequestModel model)
        {
            /*
             * ApiController valida automáticamente las anotaciones
             * del modelo, pero mantenemos esta validación para que
             * el flujo quede claro.
             */
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
             * La contraseña nunca se guarda directamente.
             * BCrypt genera un hash seguro antes de enviarla
             * al procedimiento almacenado.
             */
            string contrasennaEncriptada =
                BCrypt.Net.BCrypt.HashPassword(model.Contrasenna);

            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@nomb", model.Nombre);
            parameters.Add(
                "@primer_apellido",
                model.PrimerApellido);
            parameters.Add(
                "@segundo_apellido",
                model.SegundoApellido);
            parameters.Add(
                "@identificacion",
                model.Identificacion);
            parameters.Add("@correo", model.Correo);
            parameters.Add("@telefono", model.Telefono);
            parameters.Add("@direccion", model.Direccion);
            parameters.Add(
                "@contrasenna",
                contrasennaEncriptada);

            var resultado =
                connection.QueryFirstOrDefault
                <RegistroEstudianteResponseModel>(
                    "spRegistrarUsuarioEstudiante",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

            if (resultado == null)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new RegistroEstudianteResponseModel
                    {
                        Exitoso = false,
                        Mensaje =
                            "No fue posible completar el registro."
                    });
            }

            if (!resultado.Exitoso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}