using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RenameMessageToInstantMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameTable(name: "Message", newName: "InstantMessage");
            migrationBuilder.RenameIndex("IX_Message_ConferenceId", "IX_InstantMessage_ConferenceId", "InstantMessage");
            migrationBuilder.RenameIndex("IX_Message_TimeStamp", "IX_InstantMessage_TimeStamp", "InstantMessage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex("IX_InstantMessage_TimeStamp", "IX_Message_TimeStamp", "InstantMessage");
            migrationBuilder.RenameIndex("IX_InstantMessage_ConferenceId", "IX_Message_ConferenceId", "InstantMessage");
            migrationBuilder.RenameTable(name: "InstantMessage", newName: "Message");
        }
    }
}
