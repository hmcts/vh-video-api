using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RenameInterpreterRoomToParticipantRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "RoomParticipant",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
            
            migrationBuilder.Sql("Update Room SET Discriminator = 'ParticipantRoom' WHERE [Type] in (4, 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipant_Participant_ParticipantId",
                table: "RoomParticipant");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipant_ParticipantId",
                table: "RoomParticipant");

            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "RoomParticipant",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
