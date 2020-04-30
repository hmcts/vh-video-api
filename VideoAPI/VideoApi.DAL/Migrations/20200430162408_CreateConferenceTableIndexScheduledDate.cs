using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class CreateConferenceTableIndexScheduledDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Conference_ScheduledDateTime",
                table: "Conference",
                column: "ScheduledDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conference_ScheduledDateTime",
                table: "Conference");
        }
    }
}
