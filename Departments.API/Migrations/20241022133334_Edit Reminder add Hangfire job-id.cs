using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Departments.API.Migrations
{
    /// <inheritdoc />
    public partial class EditReminderaddHangfirejobid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HangfireJobId",
                table: "Reminders",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangfireJobId",
                table: "Reminders");
        }
    }
}
