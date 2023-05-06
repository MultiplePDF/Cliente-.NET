using System.ComponentModel.DataAnnotations;

namespace client.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }
        public ForgotPasswordModel()
        {
        }
    }
}
