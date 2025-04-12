using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class SectionModelEdit
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Необхідне назва секції")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Необхідне роз'яснення або мета секції")]
        [DataType(DataType.MultilineText)]
        [MinLength(5)]
        [MaxLength(1024)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Необхідний кольор для індентифікації")]
        public string PrimaryColor { get; set; }

        [Required(ErrorMessage = "Необхідно обрати тімліда секції")]
        public string TeamLeadId { get; set; }
        public List<SelectListItem> TeamLeadSelect { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Необхідно обрати тімліда секції")]
        public string WorkstationId { get; set; }
        public List<SelectListItem> WorkstationsSelect { get; set; } = new List<SelectListItem>();

        //[Required(ErrorMessage = "Необхідно обрати робітників до секції")]
        public IEnumerable<string> UsersWorkersId { get; set; }
        public List<SelectListItem> UsersWorkersSelect { get; set; } = new List<SelectListItem>();

        public List<TaskViewModel> Tasks { get; set; } = new();
    }
}
