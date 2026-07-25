using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IConfiguration _config;

        public AsistenciaService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<AsistenciaModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<AsistenciaModel>(
                "spListarAsistencias",
                commandType: CommandType.StoredProcedure);
        }

        public AsistenciaModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_asistencia", id);

            return connection.QueryFirstOrDefault<AsistenciaModel>(
                "spConsultarAsistencia",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(AsistenciaModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_matricula", model.IdMatricula);
            parametros.Add("@fecha", model.Fecha);
            parametros.Add("@estado", model.Estado);

            var filasAfectadas = connection.Execute(
                "spRegistrarAsistencia",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(AsistenciaModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_asistencia", model.IdAsistencia);
            parametros.Add("@id_matricula", model.IdMatricula);
            parametros.Add("@fecha", model.Fecha);
            parametros.Add("@estado", model.Estado);

            var filasAfectadas = connection.Execute(
                "spActualizarAsistencia",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_asistencia", id);

            var filasAfectadas = connection.Execute(
                "spEliminarAsistencia",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}