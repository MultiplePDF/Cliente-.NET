using System.ComponentModel.DataAnnotations;

namespace client.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(20, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres", MinimumLength = 8)]
        public string Password { get; set; }
        public LoginModel()
        {
        }
    }
}
