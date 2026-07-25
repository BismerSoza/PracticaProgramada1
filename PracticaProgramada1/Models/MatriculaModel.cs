using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class MatriculaModel
    {
        public int IdMatricula { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un estudiante.")]
        public int IdEstudiante { get; set; }

        public string? Estudiante { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un curso.")]
        public int IdCurso { get; set; }

        public string? NombreCurso { get; set; }

        public DateTime FechaMatricula { get; set; }

        public bool Estado { get; set; }
    }
}