using API.Models;

namespace API.Services
{
    public interface IProfesorService
    {
        IEnumerable<ProfesorModel> Listar();

        ProfesorModel? Consultar(int id);

        bool Actualizar(ProfesorModel model);

        bool Desactivar(int id);
    }
}