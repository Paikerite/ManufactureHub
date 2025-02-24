using ManufactureHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManufactureHub.Data
{
    public class ManufactureHubContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ManufactureHubContext(DbContextOptions<ManufactureHubContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<TaskViewModel> Tasks { get; set; }
        public DbSet<SectionViewModel> Sections { get; set; }
        public DbSet<WorkstationViewModel> Workstations { get; set; }
    }
}
