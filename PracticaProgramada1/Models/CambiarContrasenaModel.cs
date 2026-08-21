using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class CambiarContrasenaModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Ingrese su contraseña actual")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la nueva contraseña")]
        [MinLength(6, ErrorMessage = "Debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string ContrasenaNueva { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la nueva contraseña")]
        [Compare("ContrasenaNueva", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
