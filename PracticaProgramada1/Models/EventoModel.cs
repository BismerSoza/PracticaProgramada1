using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class EventoModel
    {
        public int IdEvento { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCurso { get; set; }

        public string? NombreCurso { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEvento { get; set; }

        [Required]
        public string Lugar { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}