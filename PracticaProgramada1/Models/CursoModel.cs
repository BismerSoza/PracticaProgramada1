using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class CursoModel
    {
        public int IdCurso { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un profesor.")]
        public int IdProfesor { get; set; }

        public string? Profesor { get; set; }

        [Required(ErrorMessage = "El nombre del curso es obligatorio.")]
        public string NombreCurso { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}