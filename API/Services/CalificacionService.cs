using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly IConfiguration _config;

        public CalificacionService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<CalificacionModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<CalificacionModel>(
                "spListarCalificaciones",
                commandType: CommandType.StoredProcedure);
        }

        public CalificacionModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_calificacion", id);

            return connection.QueryFirstOrDefault<CalificacionModel>(
                "spConsultarCalificacion",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(CalificacionModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_matricula", model.IdMatricula);
            parametros.Add("@nota", model.Nota);

            var filasAfectadas = connection.Execute(
                "spRegistrarCalificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(CalificacionModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_calificacion", model.IdCalificacion);
            parametros.Add("@id_matricula", model.IdMatricula);
            parametros.Add("@nota", model.Nota);

            var filasAfectadas = connection.Execute(
                "spActualizarCalificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_calificacion", id);

            var filasAfectadas = connection.Execute(
                "spEliminarCalificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}