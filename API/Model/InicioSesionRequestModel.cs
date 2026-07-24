using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class InicioSesionRequestModel
    {
        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasenna { get; set; } = string.Empty;
    }
}