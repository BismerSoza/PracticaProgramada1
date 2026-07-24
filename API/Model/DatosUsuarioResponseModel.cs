namespace API.Models
{
    public class DatosUsuarioResponseModel
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Contrasenna { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
    }
}