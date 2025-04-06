using ManufactureHub.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufactureHub.Models
{
    public class SectionViewModel
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

        public int IdTeamLead { get; set; }

        public int WorkstationId { get; set; }
        [ForeignKey("WorkstationId")]
        public WorkstationViewModel? Workstation { get; set; }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<TaskViewModel> Tasks { get; set; } = new();
    }
}
