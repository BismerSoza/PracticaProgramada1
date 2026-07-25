using API.Models;

namespace API.Services
{
    public interface INotificacionService
    {
        IEnumerable<NotificacionModel> Listar();

        NotificacionModel? Consultar(int id);

        bool Registrar(NotificacionModel model);

        bool Actualizar(NotificacionModel model);

        bool MarcarComoLeida(int id);

        bool Eliminar(int id);
    }
}