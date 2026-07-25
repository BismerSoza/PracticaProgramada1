using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class EventoService : IEventoService
    {
        private readonly IConfiguration _config;

        public EventoService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<EventoModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<EventoModel>(
                "spListarEventos",
                commandType: CommandType.StoredProcedure);
        }

        public EventoModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_evento", id);

            return connection.QueryFirstOrDefault<EventoModel>(
                "spConsultarEvento",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(EventoModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_curso", model.IdCurso);
            parametros.Add("@titulo", model.Titulo);
            parametros.Add("@descripcion", model.Descripcion);
            parametros.Add("@fecha_evento", model.FechaEvento);
            parametros.Add("@lugar", model.Lugar);

            var filasAfectadas = connection.Execute(
                "spRegistrarEvento",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(EventoModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_evento", model.IdEvento);
            parametros.Add("@id_curso", model.IdCurso);
            parametros.Add("@titulo", model.Titulo);
            parametros.Add("@descripcion", model.Descripcion);
            parametros.Add("@fecha_evento", model.FechaEvento);
            parametros.Add("@lugar", model.Lugar);
            parametros.Add("@estado", model.Estado);

            var filasAfectadas = connection.Execute(
                "spActualizarEvento",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Desactivar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_evento", id);

            var filasAfectadas = connection.Execute(
                "spDesactivarEvento",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}