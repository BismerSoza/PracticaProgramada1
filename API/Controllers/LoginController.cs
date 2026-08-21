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

        [HttpPost("cambiar-contrasena")]
        public IActionResult CambiarContrasena(
            [FromBody] CambiarContrasenaRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametrosConsulta = new DynamicParameters();
            parametrosConsulta.Add("@id_usuario", model.IdUsuario);

            var hashActual = connection.QueryFirstOrDefault<string>(
                "spObtenerContrasenaActual",
                parametrosConsulta,
                commandType: CommandType.StoredProcedure
            );

            if (hashActual == null ||
                !BCrypt.Net.BCrypt.Verify(model.ContrasenaActual, hashActual))
            {
                return Unauthorized("La contraseña actual no es correcta.");
            }

            string nuevaHash = BCrypt.Net.BCrypt.HashPassword(model.ContrasenaNueva);

            var parametrosUpdate = new DynamicParameters();
            parametrosUpdate.Add("@id_usuario", model.IdUsuario);
            parametrosUpdate.Add("@contraseña", nuevaHash);

            var filasAfectadas = connection.QueryFirstOrDefault<int>(
                "spCambiarContrasena",
                parametrosUpdate,
                commandType: CommandType.StoredProcedure
            );

            if (filasAfectadas <= 0)
                return BadRequest("No se pudo actualizar la contraseña.");

            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        [HttpPost("recuperar-acceso")]
        public async Task<IActionResult> RecuperarAcceso(
            [FromBody] RecuperarAccesoRequestModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@correo", model.Correo);

 
            var usuario = connection.QueryFirstOrDefault<DatosUsuarioResponseModel>(
                "spValidarCorreoUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (usuario == null)
                return NotFound("No fue posible validar la información.");

            string temporal = _utiles.GenerarContrasena();
            string temporalHash = BCrypt.Net.BCrypt.HashPassword(temporal);

            var parametrosUpdate = new DynamicParameters();
            parametrosUpdate.Add("@id_usuario", usuario.IdUsuario);
            parametrosUpdate.Add("@contraseña", temporalHash);

            var filasAfectadas = connection.QueryFirstOrDefault<int>(
                "spActualizarContrasennaUsuario",
                parametrosUpdate,
                commandType: CommandType.StoredProcedure
            );

            if (filasAfectadas <= 0)
                return BadRequest("No se ha podido recuperar su acceso, intente nuevamente.");

            string ruta = Path.Combine(AppContext.BaseDirectory, "Templates", "RecuperarAcceso.html");
            string plantilla = System.IO.File.ReadAllText(ruta);

            plantilla = plantilla.Replace("{{NOMBRE}}", usuario.Nombre);
            plantilla = plantilla.Replace("{{TEMPORAL}}", temporal);

            await _utiles.EnviarCorreoAsync(usuario.Correo, "Recuperación de acceso", plantilla);

            return Ok(new { mensaje = "Se ha enviado una contraseña temporal a su correo." });
        }

        [HttpPost("registro-estudiante")]
        public IActionResult RegistrarEstudiante(
            [FromBody] RegistroEstudianteRequestModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


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