using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CambiarContrasenaRequestModel
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string ContrasenaNueva { get; set; } = string.Empty;

        [Required]
        [Compare("ContrasenaNueva", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
