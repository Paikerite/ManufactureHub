using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManufactureHub.Migrations
{
    /// <inheritdoc />
    public partial class changedRelationInTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionViewModelTaskViewModel");

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SectionId",
                table: "Tasks",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Sections_SectionId",
                table: "Tasks",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Sections_SectionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SectionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "SectionViewModelTaskViewModel",
                columns: table => new
                {
                    SectionsId = table.Column<int>(type: "int", nullable: false),
                    TasksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionViewModelTaskViewModel", x => new { x.SectionsId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_SectionViewModelTaskViewModel_Sections_SectionsId",
                        column: x => x.SectionsId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionViewModelTaskViewModel_Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionViewModelTaskViewModel_TasksId",
                table: "SectionViewModelTaskViewModel",
                column: "TasksId");
        }
    }
}
