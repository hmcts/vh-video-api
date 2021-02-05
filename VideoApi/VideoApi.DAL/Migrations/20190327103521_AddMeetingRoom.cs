using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddMeetingRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUri",
                table: "Conference",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JudgeUri",
                table: "Conference",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantUri",
                table: "Conference",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PexipNode",
                table: "Conference",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUri",
                table: "Conference");

            migrationBuilder.DropColumn(
                name: "JudgeUri",
                table: "Conference");

            migrationBuilder.DropColumn(
                name: "ParticipantUri",
                table: "Conference");

            migrationBuilder.DropColumn(
                name: "PexipNode",
                table: "Conference");
        }
    }
}
