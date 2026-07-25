namespace API.Models
{
    public class CalificacionModel
    {
        public int IdCalificacion { get; set; }

        public int IdMatricula { get; set; }

        public string? Estudiante { get; set; }

        public string? NombreCurso { get; set; }

        public decimal Nota { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}