using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не вказан Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Не вказан пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
