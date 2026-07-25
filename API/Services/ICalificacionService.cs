using API.Models;

namespace API.Services
{
    public interface ICalificacionService
    {
        IEnumerable<CalificacionModel> Listar();

        CalificacionModel? Consultar(int id);

        bool Registrar(CalificacionModel model);

        bool Actualizar(CalificacionModel model);

        bool Eliminar(int id);
    }
}