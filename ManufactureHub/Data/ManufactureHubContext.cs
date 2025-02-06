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

        //public DbSet<ClassViewModel> Classes { get; set; }
        //public DbSet<LessonViewModel> Lessons { get; set; }
        //public DbSet<UserAccountViewModel> Users { get; set; }
    }
}
