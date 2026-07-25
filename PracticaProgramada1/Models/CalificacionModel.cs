using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class CalificacionModel
    {
        public int IdCalificacion { get; set; }

        [Range(1, int.MaxValue)]
        public int IdMatricula { get; set; }

        public string? Estudiante { get; set; }

        public string? NombreCurso { get; set; }

        [Range(0, 100,
            ErrorMessage = "La nota debe estar entre 0 y 100.")]
        public decimal Nota { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}