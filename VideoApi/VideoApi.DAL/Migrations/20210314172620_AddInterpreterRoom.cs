using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddInterpreterRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Room_CurrentVirtualRoomId",
                table: "Endpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Room_CurrentVirtualRoomId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_CurrentVirtualRoomId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_CurrentVirtualRoomId",
                table: "Endpoint");

            migrationBuilder.RenameColumn(
                name: "CurrentVirtualRoomId",
                table: "Participant",
                "CurrentConsultationRoomId");

            migrationBuilder.RenameColumn(
                name: "CurrentVirtualRoomId",
                table: "Endpoint",
                "CurrentConsultationRoomId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Room",
                defaultValue:"ConsultationRoom",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_CurrentConsultationRoomId",
                table: "Participant",
                column: "CurrentConsultationRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_CurrentConsultationRoomId",
                table: "Endpoint",
                column: "CurrentConsultationRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Room_CurrentConsultationRoomId",
                table: "Endpoint",
                column: "CurrentConsultationRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Room_CurrentConsultationRoomId",
                table: "Participant",
                column: "CurrentConsultationRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("Update Room SET Discriminator = 'InterpreterRoom' WHERE [Type] in (4, 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Room_CurrentConsultationRoomId",
                table: "Endpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Room_CurrentConsultationRoomId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_CurrentConsultationRoomId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_CurrentConsultationRoomId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Room");

            migrationBuilder.RenameColumn(
                name: "CurrentConsultationRoomId",
                table: "Participant",
                "CurrentVirtualRoomId");

            migrationBuilder.RenameColumn(
                name: "CurrentConsultationRoomId",
                table: "Endpoint",
                "CurrentVirtualRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_CurrentVirtualRoomId",
                table: "Participant",
                column: "CurrentVirtualRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_CurrentVirtualRoomId",
                table: "Endpoint",
                column: "CurrentVirtualRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Room_CurrentVirtualRoomId",
                table: "Endpoint",
                column: "CurrentVirtualRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Room_CurrentVirtualRoomId",
                table: "Participant",
                column: "CurrentVirtualRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
