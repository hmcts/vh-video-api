using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddCurrentVirtualRoomToParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentVirtualRoomId",
                table: "Participant",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_CurrentVirtualRoomId",
                table: "Participant",
                column: "CurrentVirtualRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Room_CurrentVirtualRoomId",
                table: "Participant",
                column: "CurrentVirtualRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Room_CurrentVirtualRoomId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_CurrentVirtualRoomId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "CurrentVirtualRoomId",
                table: "Participant");
        }
    }
}
