using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSwimlaneToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Swimlane",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_Date_Swimlane",
                table: "tasks",
                columns: new[] { "Date", "Swimlane" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tasks_Date_Swimlane",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Swimlane",
                table: "tasks");
        }
    }
}
