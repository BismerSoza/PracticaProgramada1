namespace API.Models
{
    public class EventoModel
    {
        public int IdEvento { get; set; }

        public int IdCurso { get; set; }

        public string? NombreCurso { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public DateTime FechaEvento { get; set; }

        public string Lugar { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}