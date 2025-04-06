using ManufactureHub.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class RegisterModel
    {
        //[Key]
        //public int Id { get; set; }

        [Required(ErrorMessage = "Не вказано ім'я")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не вказана фамілія")]
        public string? SurName { get; set; }

        [Required(ErrorMessage = "Не вказано прізвище по батькові")]
        public string? PatronymicName { get; set; }

        //public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "Будь ласка, оберіть роль в системі")]
        [EnumDataType(typeof(Roles), ErrorMessage = "Будь ласка, оберіть роль в системі з переліку")]
        public Roles Role { get; set; }

        [Required(ErrorMessage = "Не вказан Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Не вказана посада")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Position { get; set; } // Посада (наприклад, "Системний адміністратор")

        //public DateTime EmploymentDate { get; set; } // Дата прийому на роботу

        [Required(ErrorMessage = "Не вказан пароль")]
        [StringLength(50, ErrorMessage = "Мінімальна кількість символів - 6, а максимальна - 50", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get;set; }

        //public List<ClassViewModel> Classes { get; set; } = new();
    }
}
