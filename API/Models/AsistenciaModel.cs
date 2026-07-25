namespace API.Models
{
    public class AsistenciaModel
    {
        public int IdAsistencia { get; set; }

        public int IdMatricula { get; set; }

        public string? Estudiante { get; set; }

        public string? NombreCurso { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}