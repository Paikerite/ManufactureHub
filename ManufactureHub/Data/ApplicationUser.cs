using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Display(Name="Ім`я")]
        [Required(ErrorMessage = "Не вказано ім'я")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }
        [Display(Name = "Фамілія")]
        [Required(ErrorMessage = "Не вказана фамілія")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string SurName { get; set; }
        [Display(Name="По-батькові")]
        [Required(ErrorMessage = "Не вказано прізвище по батькові")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string PatronymicName { get; set; }
        public string? ProfilePicture { get; set; }

        //[Required(ErrorMessage = "Не вказано до якого виробництво відноситься")]
        //[MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        //public string Department { get; set; } // Відділ (наприклад, "Виробництво", "IT", "Бухгалтерія")

        [Display(Name = "Позиція")]
        [Required(ErrorMessage = "Не вказана посада")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Position { get; set; } // Посада (наприклад, "Системний адміністратор")

        [Display(Name = "Дата прийому")]
        [DataType(DataType.Date)]
        public DateTime EmploymentDate { get; set; } // Дата прийому на роботу

        // Права доступу та ролі
        public bool IsActive { get; set; } // Статус користувача (активний чи ні)

        // Логування дій (якщо потрібно)
        public DateTime LastLoginDate { get; set; } // Останній вхід в систему
        public string LastLoginIP { get; set; } // IP-адреса останнього входу

        public List<SectionViewModel> Sections { get; set; } = new();
    }
}
