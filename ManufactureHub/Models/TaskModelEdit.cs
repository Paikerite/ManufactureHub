using ManufactureHub.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufactureHub.Models
{
    public class TaskModelEdit
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідна назва завдання")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідний опис завдання")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Необхідно назначити дедлайн")]
        public DateTime Deadline { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [Required(ErrorMessage = "Необхідно назначити пріоритет")]
        public Priority Priority { get; set; }

        public StatusTask StatusTask { get; set; }

        public string? ProfilePictureUploader { get; set; }

        public string? FileUrl { get; set; }

        public int SectionId { get; set; }
        [ForeignKey("SectionId")]
        public SectionViewModel? Section { get; set; }
    }
}
