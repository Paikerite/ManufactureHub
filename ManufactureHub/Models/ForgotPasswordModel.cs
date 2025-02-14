using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
