using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddConferenceClosedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDateTime",
                table: "Conference",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_TestCallResultId",
                table: "Participant",
                column: "TestCallResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_TestCallResult_TestCallResultId",
                table: "Participant",
                column: "TestCallResultId",
                principalTable: "TestCallResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_TestCallResult_TestCallResultId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_TestCallResultId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "ClosedDateTime",
                table: "Conference");
        }
    }
}
