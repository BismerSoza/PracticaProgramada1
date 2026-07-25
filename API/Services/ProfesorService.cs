using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly IConfiguration _config;

        public ProfesorService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<ProfesorModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<ProfesorModel>(
                "spListarProfesores",
                commandType: CommandType.StoredProcedure);
        }

        public ProfesorModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_profesor", id);

            return connection.QueryFirstOrDefault<ProfesorModel>(
                "spConsultarProfesor",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Actualizar(ProfesorModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_profesor", model.IdProfesor);
            parametros.Add("@nomb", model.Nombre);
            parametros.Add("@primer_apellido", model.PrimerApellido);
            parametros.Add("@segundo_apellido", model.SegundoApellido);
            parametros.Add("@identificacion", model.Identificacion);
            parametros.Add("@correo", model.Correo);
            parametros.Add("@telefono", model.Telefono);
            parametros.Add("@especialidad", model.Especialidad);

            var filasAfectadas = connection.Execute(
                "spActualizarProfesor",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Desactivar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_profesor", id);

            var filasAfectadas = connection.Execute(
                "spDesactivarProfesor",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}