using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManufactureHub.Migrations
{
    /// <inheritdoc />
    public partial class madeOneToManySectionToWorkstation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Workstations_WorkstationViewModelId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_WorkstationViewModelId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "WorkstationViewModelId",
                table: "Sections");

            migrationBuilder.AddColumn<int>(
                name: "WorkstationId",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_WorkstationId",
                table: "Sections",
                column: "WorkstationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Workstations_WorkstationId",
                table: "Sections",
                column: "WorkstationId",
                principalTable: "Workstations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Workstations_WorkstationId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_WorkstationId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "WorkstationId",
                table: "Sections");

            migrationBuilder.AddColumn<int>(
                name: "WorkstationViewModelId",
                table: "Sections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_WorkstationViewModelId",
                table: "Sections",
                column: "WorkstationViewModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Workstations_WorkstationViewModelId",
                table: "Sections",
                column: "WorkstationViewModelId",
                principalTable: "Workstations",
                principalColumn: "Id");
        }
    }
}
