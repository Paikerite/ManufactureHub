using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required(ErrorMessage = "Не вказано ім'я")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не вказана фамілія")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Не вказано прізвище по батькові")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string PatronymicName { get; set; }
        public string? ProfilePicture { get; set; }

        //[Required(ErrorMessage = "Не вказано до якого виробництво відноситься")]
        //[MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        //public string Department { get; set; } // Відділ (наприклад, "Виробництво", "IT", "Бухгалтерія")

        [Required(ErrorMessage = "Не вказана посада")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Position { get; set; } // Посада (наприклад, "Системний адміністратор")

        public DateTime EmploymentDate { get; set; } // Дата прийому на роботу

        // Права доступу та ролі
        public bool IsActive { get; set; } = true; // Статус користувача (активний чи ні)

        // Логування дій (якщо потрібно)
        public DateTime LastLoginDate { get; set; } // Останній вхід в систему
        public string LastLoginIP { get; set; } // IP-адреса останнього входу

        public List<SectionViewModel> Sections { get; set; }
    }
}
