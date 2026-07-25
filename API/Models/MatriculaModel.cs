namespace API.Models
{
    public class MatriculaModel
    {
        public int IdMatricula { get; set; }

        public int IdEstudiante { get; set; }

        public string? Estudiante { get; set; }

        public int IdCurso { get; set; }

        public string? NombreCurso { get; set; }

        public DateTime FechaMatricula { get; set; }

        public bool Estado { get; set; }
    }
}