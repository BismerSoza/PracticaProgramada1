namespace API.Models
{
    public class RegistroEstudianteResponseModel
    {
        public bool Exitoso { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public int IdEstudiante { get; set; }
    }
}