using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ManufactureHub.Migrations
{
    /// <inheritdoc />
    public partial class statusInTaskAndPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUploader",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusTask",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUploader",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StatusTask",
                table: "Tasks");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName", "RoleEnum", "RoleName" },
                values: new object[,]
                {
                    { 1, null, "Відповідає за управління, контроль та підтримку роботи системи, організації, мережі або інфраструктури. Також, відповідає за функціонування веб-сайту, включаючи оновлення контенту, технічну підтримку та забезпечення безпеки", "Admin", null, 5, "Адмін" },
                    { 2, null, "Відповідає за організацію, управління та контроль виробничих процесів на підприємстві. Є ключовою в галузі виробництва, оскільки директор виробництва забезпечує ефективне функціонування всіх виробничих підрозділів, дотримання технологічних стандартів, своєчасне виконання замовлень та досягнення встановлених цілей.", "HeadFacility", null, 4, "Директор виробництва" },
                    { 3, null, "Займається організацією та оптимізацією потоків матеріалів, сировини, готової продукції та інших ресурсів, необхідних для ефективного функціонування виробництва. Основна мета логістика виробництва полягає в забезпеченні безперебійного постачання, мінімізації витрат та підвищенні ефективності виробничих процесів.", "LogisticTeam", null, 3, "Логістика" },
                    { 4, null, "Керує роботою цеху, відповідає за організацію виробничих процесів, виконання планових завдань, якість продукції та ефективне використання ресурсів. Є ключовою в структурі виробництва, оскільки керівник цеху безпосередньо впливає на результативність роботи підрозділу та досягнення загальних цілей підприємства.", "TeamLeadWorkstation", null, 2, "Керівник цеху" },
                    { 5, null, "Відповідає за організацію та управління роботою конкретної дільниці (виробничої зони, цеху або відділу) у межах підприємства. Забезпечує виконання виробничих завдань, дотримання технологічних процесів, контроль якості продукції та ефективне використання ресурсів на своїй дільниці.", "TeamLeadSection", null, 1, "Керівник дільниці" },
                    { 6, null, "Забезпечує процес обробки деталей на верстатах з програмним керуванням.", "Worker", null, 0, "Робітник" }
                });
        }
    }
}
