using System.ComponentModel.DataAnnotations;

namespace PracticaProgramada1.Models
{
    public class RecuperarModel
    {
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido")]
        public string Correo { get; set; } = string.Empty;
    }
}