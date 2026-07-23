namespace PracticaProgramada1.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Contrasenna { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}

/*public class UsuarioModel
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Contrasenna { get; set; } = string.Empty;
    public string ConfirmeSuContrasenna { get; set; } = string.Empty;

}
}*/
