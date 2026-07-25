using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class NotificacionModel
    {
        public int IdNotificacion { get; set; }

        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }

        public string? Correo { get; set; }

        [Required]
        public string Asunto { get; set; } = string.Empty;

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        public bool Leida { get; set; }

        public DateTime FechaEnvio { get; set; }

        public DateTime? FechaLectura { get; set; }
    }
}