using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class WorkstationViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Необхідне назва секції")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Необхідне роз'яснення або мета цеху")]
        public string Description { get; set; }
        public int IdTeamLead { get; set; }

        public List<SectionViewModel> Sections { get; set; } = new();
    }
}
