using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RemoveTasksFromConference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alert_Conference_ConferenceId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Alert_ConferenceId",
                table: "Task");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConferenceId",
                table: "Task",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ConferenceId",
                table: "Task",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_Alert_ConferenceId",
                table: "Task",
                column: "ConferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alert_Conference_ConferenceId",
                table: "Task",
                column: "ConferenceId",
                principalTable: "Conference",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
