using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class WorkstationModelPost
    {
        [Required(ErrorMessage = "Необхідне назва цеху")]
        [MaxLength(256, ErrorMessage = "Максимальна кількість символів 256")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Необхідне роз'яснення або мета цеху")]
        [DataType(DataType.MultilineText)]
        [MinLength(5)]
        [MaxLength(1024)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Необхідно обрати тімліда цеху")]
        public string TeamLeadId { get; set; }
        public List<SelectListItem> TeamLeadSelect { get; set; } = new List<SelectListItem>();
    }
}
