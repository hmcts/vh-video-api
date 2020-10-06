using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class PopulateNullHearingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Claimant LIP' WHERE UserRole = 6 and HearingRole IS NULL AND CaseTypeGroup = 'Claimant'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Defendant LIP' WHERE UserRole = 6 and HearingRole IS NULL AND CaseTypeGroup = 'Defendant'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Claimant' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'Claimant'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Defendant' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'Defendant'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Observer' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'Observer'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'PanelMember' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'PanelMember'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Applicant' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'Applicant'");
            migrationBuilder.Sql("UPDATE dbo.Participant SET HearingRole = 'Respondent' WHERE UserRole = 5 and HearingRole IS NULL AND CaseTypeGroup = 'Respondent'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
