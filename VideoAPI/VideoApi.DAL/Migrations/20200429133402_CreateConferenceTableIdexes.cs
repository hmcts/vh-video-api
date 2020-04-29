using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class CreateConferenceTableIdexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Conference_HearingRefId",
                table: "Conference",
                column: "HearingRefId");
            
            migrationBuilder.CreateIndex(
                name: "IX_Conference_State",
                table: "Conference",
                column: "State");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conference_HearingRefId",
                table: "Conference");
            
            migrationBuilder.DropIndex(
                name: "IX_Conference_State",
                table: "Conference");
        }
    }
}
