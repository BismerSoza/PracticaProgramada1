using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ErrorController(IConfiguration config)
        {
            _config = config;
        }

        [Route("RegistrarError")]
        public IActionResult RegistrarError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            using var context = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@mensaje_error", exception?.Error.Message);
            parameters.Add("@lugar", exception?.Path);
            parameters.Add("@stack_trace", exception?.Error.StackTrace);
            parameters.Add("@id_usuario", 0);

            context.Execute("spRegistrarError", parameters);
            return StatusCode(500, "Se presentó un inconveniente técnico");
        }
    }
}