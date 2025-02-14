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

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 1,
                RoleName = "Адмін",
                Name = "Admin",
                Description = "Відповідає за управління, контроль та підтримку роботи системи, організації, мережі або інфраструктури. Також, відповідає за функціонування веб-сайту, включаючи оновлення контенту, технічну підтримку та забезпечення безпеки",
                RoleEnum = Enums.Roles.Admin,
            });

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 2,
                RoleName = "Директор виробництва",
                Name = "HeadFacility",
                Description = "Відповідає за організацію, управління та контроль виробничих процесів на підприємстві. Є ключовою в галузі виробництва, оскільки директор виробництва забезпечує ефективне функціонування всіх виробничих підрозділів, дотримання технологічних стандартів, своєчасне виконання замовлень та досягнення встановлених цілей.",
                RoleEnum = Enums.Roles.HeadFacility,
            });

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 3,
                RoleName = "Логістика",
                Name = "LogisticTeam",
                Description = "Займається організацією та оптимізацією потоків матеріалів, сировини, готової продукції та інших ресурсів, необхідних для ефективного функціонування виробництва. Основна мета логістика виробництва полягає в забезпеченні безперебійного постачання, мінімізації витрат та підвищенні ефективності виробничих процесів.",
                RoleEnum = Enums.Roles.LogisticTeam,
            });

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 4,
                RoleName = "Керівник цеху",
                Name = "TeamLeadWorkstation",
                Description = "Керує роботою цеху, відповідає за організацію виробничих процесів, виконання планових завдань, якість продукції та ефективне використання ресурсів. Є ключовою в структурі виробництва, оскільки керівник цеху безпосередньо впливає на результативність роботи підрозділу та досягнення загальних цілей підприємства.",
                RoleEnum = Enums.Roles.TeamLeadWorkstation,
            });

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 5,
                RoleName = "Керівник дільниці",
                Name = "TeamLeadSection",
                Description = "Відповідає за організацію та управління роботою конкретної дільниці (виробничої зони, цеху або відділу) у межах підприємства. Забезпечує виконання виробничих завдань, дотримання технологічних процесів, контроль якості продукції та ефективне використання ресурсів на своїй дільниці.",
                RoleEnum = Enums.Roles.TeamLeadSection,
            });

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = 6,
                RoleName = "Робітник",
                Name = "Worker",
                Description = "Забезпечує процес обробки деталей на верстатах з програмним керуванням.",
                RoleEnum = Enums.Roles.Worker,
            });

        }

        public DbSet<TaskViewModel> Tasks { get; set; }
        public DbSet<SectionViewModel> Sections { get; set; }
        public DbSet<WorkstationViewModel> Workstations { get; set; }
    }
}
