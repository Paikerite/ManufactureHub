using ManufactureHub.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufactureHub.Models
{
    public class TaskModelPost
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідна назва завдання")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необхідний опис завдання")]
        [DataType(DataType.MultilineText)]
        [MinLength(5)]
        [MaxLength(1024)]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Необхідно назначити дедлайн")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Необхідно назначити пріоритет")]
        public Priority Priority { get; set; }

        [Display(Name = "File")]
        public IFormFile? FormFile { get; set; }

        [Required(ErrorMessage = "Необхідно обрати тімліда секції")]
        public string SectionId { get; set; }
        public List<SelectListItem> SectionSelect { get; set; } = new List<SelectListItem>();
    }
}
