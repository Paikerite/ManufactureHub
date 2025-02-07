using ManufactureHub.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class TaskViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідна назва завдання")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідний опис завдання")]
        public string Description { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Необхідно назначити дедлайн")]
        public DateTime Deadline { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [Required(ErrorMessage = "Необхідно назначити пріоритет")]
        public Priority Priority { get; set; }

        public string? FileUrl { get; set; }

        public List<SectionViewModel> Sections { get; set; } = new();
    }
}
