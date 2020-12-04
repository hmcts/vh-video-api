using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class UpdateUserRoleForWingersAndPanelMembersToJudicialOfficeHolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Participant SET UserRole = 7 
                WHERE HearingRole = 'Winger' OR HearingRole = 'Panel Member'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Method intentionally left empty
        }
    }
}
