using API.Models;

namespace API.Services
{
    public interface ICursoService
    {
        IEnumerable<CursoModel> Listar();

        CursoModel? Consultar(int id);

        bool Registrar(CursoModel model);

        bool Actualizar(CursoModel model);

        bool Desactivar(int id);
    }
}