using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddTansferRoomLabelToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransferredFromRoomLabel",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferredToRoomLabel",
                table: "Event",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferredFromRoomLabel",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "TransferredToRoomLabel",
                table: "Event");
        }
    }
}
