using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Models
{
    public class EditUserModelByWorker
    {
        [Key]
        public int Id { get; set; }

        public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "Не вказан пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
