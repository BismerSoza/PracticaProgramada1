using API.Models;

namespace API.Services
{
    public interface IAsistenciaService
    {
        IEnumerable<AsistenciaModel> Listar();

        AsistenciaModel? Consultar(int id);

        bool Registrar(AsistenciaModel model);

        bool Actualizar(AsistenciaModel model);

        bool Eliminar(int id);
    }
}