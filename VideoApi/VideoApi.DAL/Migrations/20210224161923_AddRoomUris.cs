using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddRoomUris : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IngestUrl",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantUri",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PexipNode",
                table: "Room",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngestUrl",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "ParticipantUri",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "PexipNode",
                table: "Room");
        }
    }
}
