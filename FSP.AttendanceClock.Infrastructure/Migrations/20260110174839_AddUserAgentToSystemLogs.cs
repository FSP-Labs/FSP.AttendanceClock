using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.AttendanceClock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAgentToSystemLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "SystemLogs",
                type: "text",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "SystemLogs");
        }
    }
}
