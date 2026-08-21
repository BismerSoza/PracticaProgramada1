namespace API.Services
{
    public interface IUtilesService
    {
        string GenerarToken(int idUsuario);

        string GenerarContrasena();

        Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);
    }
}