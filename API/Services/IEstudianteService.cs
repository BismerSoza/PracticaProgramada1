using API.Models;

namespace API.Services
{
    public interface IEstudianteService
    {
        IEnumerable<EstudianteModel> Listar();

        EstudianteModel? Consultar(int id);

        bool Actualizar(EstudianteModel model);

        bool Desactivar(int id);

        bool Activar(int id);
    }
}