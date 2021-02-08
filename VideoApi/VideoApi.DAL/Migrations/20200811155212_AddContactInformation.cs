using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddContactInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Participant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTelephone",
                table: "Participant",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "ContactTelephone",
                table: "Participant");
        }
    }
}
