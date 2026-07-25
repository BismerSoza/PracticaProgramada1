namespace API.Models
{
    public class NotificacionModel
    {
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }

        public string? Correo { get; set; }

        public string Asunto { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public bool Leida { get; set; }

        public DateTime FechaEnvio { get; set; }

        public DateTime? FechaLectura { get; set; }
    }
}