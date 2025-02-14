using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Не вказан пароль")]
        [StringLength(50, ErrorMessage = "Мінімальна кількість символів - 6, а максимальна - 50", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public string? Code { get; set; }
    }
}
