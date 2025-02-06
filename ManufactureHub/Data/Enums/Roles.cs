using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Data.Enums
{
    public enum Roles
    {
        [Display(Name = "Робітник")]
        Worker,
        [Display(Name = "Керівник дільниці")]
        TeamLeadSection,
        [Display(Name = "Керівник цеху")]
        TeamLeadWorkstation,
        [Display(Name = "Логістика")]
        LogisticTeam,
        [Display(Name = "Директор виробництва")]
        HeadFacility,
        [Display(Name = "Адмін")]
        Admin
    }
}
