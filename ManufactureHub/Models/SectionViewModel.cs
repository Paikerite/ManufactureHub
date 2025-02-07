using ManufactureHub.Data;
using System.ComponentModel.DataAnnotations;

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
        public string Description { get; set; }
        public string PrimaryColor { get; set; }
        public int IdTeamLead { get; set; }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<TaskViewModel> Tasks { get; set; } = new();
    }
}
