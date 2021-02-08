using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class UpdateOnDeleteMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantStatus_Participant_ParticipantId",
                table: "ParticipantStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantStatus_Participant_ParticipantId",
                table: "ParticipantStatus",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantStatus_Participant_ParticipantId",
                table: "ParticipantStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantStatus_Participant_ParticipantId",
                table: "ParticipantStatus",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
