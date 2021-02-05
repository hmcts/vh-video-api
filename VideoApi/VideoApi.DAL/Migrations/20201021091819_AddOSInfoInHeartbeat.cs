using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddOSInfoInHeartbeat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystemVersion",
                table: "Heartbeat",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OperatingSystemVersion",
                table: "Heartbeat");
        }
    }
}
