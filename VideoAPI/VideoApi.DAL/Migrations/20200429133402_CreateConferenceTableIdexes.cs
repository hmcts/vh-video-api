using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class CreateConferenceTableIdexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Conference_HearingRefId_State",
                table: "Conference",
                columns: new[]{"HearingRefId", "State"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conference_HearingRefId_State",
                table: "Conference");
        }
    }
}
