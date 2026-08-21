using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RecuperarAccesoRequestModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;
    }
}