using API.Models;

namespace API.Services
{
    public interface IEventoService
    {
        IEnumerable<EventoModel> Listar();

        EventoModel? Consultar(int id);

        bool Registrar(EventoModel model);

        bool Actualizar(EventoModel model);

        bool Desactivar(int id);
    }
}