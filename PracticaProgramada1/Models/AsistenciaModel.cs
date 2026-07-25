using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class AsistenciaModel
    {
        public int IdAsistencia { get; set; }

        [Range(1, int.MaxValue)]
        public int IdMatricula { get; set; }

        public string? Estudiante { get; set; }

        public string? NombreCurso { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}