using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegistroEstudianteRequestModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [StringLength(30, ErrorMessage = "El primer apellido no puede superar los 30 caracteres.")]
        public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El segundo apellido no puede superar los 30 caracteres.")]
        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe contener al menos 6 caracteres.")]
        [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Contrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare(nameof(Contrasenna), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;
    }
}