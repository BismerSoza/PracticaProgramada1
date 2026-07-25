using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IConfiguration _config;

        public EstudianteService(
            IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<EstudianteModel> Listar()
        {
            using var connection =
                new SqlConnection(
                    _config.GetConnectionString(
                        "DefaultConnection"));

            return connection.Query<EstudianteModel>(
                "spListarEstudiantes",
                commandType:
                    CommandType.StoredProcedure);
        }

        public EstudianteModel? Consultar(int id)
        {
            using var connection =
                new SqlConnection(
                    _config.GetConnectionString(
                        "DefaultConnection"));

            var parametros =
                new DynamicParameters();

            parametros.Add(
                "@id_estudiante",
                id);

            return connection
                .QueryFirstOrDefault<EstudianteModel>(
                    "spConsultarEstudiante",
                    parametros,
                    commandType:
                        CommandType.StoredProcedure);
        }

        public bool Actualizar(
            EstudianteModel model)
        {
            using var connection =
                new SqlConnection(
                    _config.GetConnectionString(
                        "DefaultConnection"));

            var parametros =
                new DynamicParameters();

            parametros.Add(
                "@id_estudiante",
                model.IdEstudiante);

            parametros.Add(
                "@nomb",
                model.Nombre);

            parametros.Add(
                "@primer_apellido",
                model.PrimerApellido);

            parametros.Add(
                "@segundo_apellido",
                string.IsNullOrWhiteSpace(
                    model.SegundoApellido)
                    ? null
                    : model.SegundoApellido);

            parametros.Add(
                "@identificacion",
                model.Identificacion);

            parametros.Add(
                "@correo",
                model.Correo);

            parametros.Add(
                "@telefono",
                model.Telefono);

            parametros.Add(
                "@direccion",
                model.Direccion);

            var filasAfectadas =
                connection.QuerySingle<int>(
                    "spActualizarEstudiante",
                    parametros,
                    commandType:
                        CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Desactivar(int id)
        {
            using var connection =
                new SqlConnection(
                    _config.GetConnectionString(
                        "DefaultConnection"));

            var parametros =
                new DynamicParameters();

            parametros.Add(
                "@id_estudiante",
                id);

            var filasAfectadas =
                connection.QuerySingle<int>(
                    "spDesactivarEstudiante",
                    parametros,
                    commandType:
                        CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Activar(int id)
        {
            using var connection =
                new SqlConnection(
                    _config.GetConnectionString(
                        "DefaultConnection"));

            var parametros =
                new DynamicParameters();

            parametros.Add(
                "@id_estudiante",
                id);

            var filasAfectadas =
                connection.QuerySingle<int>(
                    "spActivarEstudiante",
                    parametros,
                    commandType:
                        CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}