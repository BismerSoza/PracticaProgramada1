namespace API.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Contrasenna { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
