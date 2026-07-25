using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegistroUsuarioModel
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Contrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es requerido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es requerida")]
        public string Identificacion { get; set; } = string.Empty;

        public int IdRol { get; set; } = 1; // Rol por defecto
    }

    public class ActualizarContrasennaModel
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MinLength(8)]
        public string ContrasennaActual { get; set; } = string.Empty;
    }
}
