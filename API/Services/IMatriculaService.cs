using API.Models;

namespace API.Services
{
    public interface IMatriculaService
    {
        IEnumerable<MatriculaModel> Listar();

        MatriculaModel? Consultar(int id);

        bool Registrar(MatriculaModel model);

        bool Actualizar(MatriculaModel model);

        bool Desactivar(int id);
    }
}