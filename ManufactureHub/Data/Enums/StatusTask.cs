using System.ComponentModel.DataAnnotations;

namespace ManufactureHub.Data.Enums
{
    public enum StatusTask
    {
        [Display(Name = "Під розглядом")]
        underreview,
        [Display(Name = "В процесі")]
        inprogress,
        [Display(Name = "Виконано")]
        done,
        [Display(Name = "Відмовлено")]
        rejected,
    }
}
