using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddFirstLastNameToParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Participant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Participant",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Participant");
        }
    }
}
