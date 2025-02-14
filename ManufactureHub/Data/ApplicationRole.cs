using ManufactureHub.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace ManufactureHub.Data
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public Roles RoleEnum { get; set; }
    }
}
