using System.ComponentModel.DataAnnotations;

namespace client.Models
{
    public class UserEditModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre de usuario debe tener entre 3 y 20 caracteres", MinimumLength = 3)]
        public string Username { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }
        public UserEditModel()
        {
        }
    }
}
