using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Data.Enums
{
    public enum Priority
    {
        [Display(Name = "Низький пріоритет")]
        Low,
        [Display(Name = "Середній пріоритет")]
        Medium,
        [Display(Name = "Високий пріоритет")]
        High,
    }
}
