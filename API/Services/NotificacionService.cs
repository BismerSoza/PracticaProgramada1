using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IConfiguration _config;

        public NotificacionService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<NotificacionModel> Listar()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            return connection.Query<NotificacionModel>(
                "spListarNotificaciones",
                commandType: CommandType.StoredProcedure);
        }

        public NotificacionModel? Consultar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_notificacion", id);

            return connection.QueryFirstOrDefault<NotificacionModel>(
                "spConsultarNotificacion",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public bool Registrar(NotificacionModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_usuario", model.IdUsuario);
            parametros.Add("@asunto", model.Asunto);
            parametros.Add("@mensaje", model.Mensaje);

            var filasAfectadas = connection.Execute(
                "spRegistrarNotificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Actualizar(NotificacionModel model)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();

            parametros.Add("@id_notificacion", model.IdNotificacion);
            parametros.Add("@id_usuario", model.IdUsuario);
            parametros.Add("@asunto", model.Asunto);
            parametros.Add("@mensaje", model.Mensaje);

            var filasAfectadas = connection.Execute(
                "spActualizarNotificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool MarcarComoLeida(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_notificacion", id);

            var filasAfectadas = connection.Execute(
                "spMarcarNotificacionLeida",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }

        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@id_notificacion", id);

            var filasAfectadas = connection.Execute(
                "spEliminarNotificacion",
                parametros,
                commandType: CommandType.StoredProcedure);

            return filasAfectadas > 0;
        }
    }
}