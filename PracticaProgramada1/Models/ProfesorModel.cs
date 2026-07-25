using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class ProfesorModel
    {
        public int IdProfesor { get; set; }

        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        public string PrimerApellido { get; set; } = string.Empty;

        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public string Especialidad { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}