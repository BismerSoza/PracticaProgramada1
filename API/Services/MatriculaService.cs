using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IConfiguration _config;

        public MatriculaService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<MatriculaModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<MatriculaModel>(
                "spListarMatriculas",
                commandType: CommandType.StoredProcedure);
        }

        public MatriculaModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_matricula", id);

            return connection.QueryFirstOrDefault<MatriculaModel>(
                "spConsultarMatricula",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(MatriculaModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_estudiante", model.IdEstudiante);
            parametros.Add("@id_curso", model.IdCurso);

            var filasAfectadas = connection.Execute(
                "spRegistrarMatricula",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(MatriculaModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_matricula", model.IdMatricula);
            parametros.Add("@id_estudiante", model.IdEstudiante);
            parametros.Add("@id_curso", model.IdCurso);
            parametros.Add("@estado", model.Estado);

            var filasAfectadas = connection.Execute(
                "spActualizarMatricula",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Desactivar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_matricula", id);

            var filasAfectadas = connection.Execute(
                "spDesactivarMatricula",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}