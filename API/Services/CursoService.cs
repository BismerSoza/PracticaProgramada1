using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class CursoService : ICursoService
    {
        private readonly IConfiguration _config;

        public CursoService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<CursoModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<CursoModel>(
                "spListarCursos",
                commandType: CommandType.StoredProcedure);
        }

        public CursoModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_curso", id);

            return connection.QueryFirstOrDefault<CursoModel>(
                "spConsultarCurso",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(CursoModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_profesor", model.IdProfesor);
            parametros.Add("@nombre_curso", model.NombreCurso);
            parametros.Add("@descripcion", model.Descripcion);

            var filasAfectadas = connection.Execute(
                "spRegistrarCurso",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(CursoModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_curso", model.IdCurso);
            parametros.Add("@id_profesor", model.IdProfesor);
            parametros.Add("@nombre_curso", model.NombreCurso);
            parametros.Add("@descripcion", model.Descripcion);
            parametros.Add("@estado", model.Estado);

            var filasAfectadas = connection.Execute(
                "spActualizarCurso",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Desactivar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_curso", id);

            var filasAfectadas = connection.Execute(
                "spDesactivarCurso",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}