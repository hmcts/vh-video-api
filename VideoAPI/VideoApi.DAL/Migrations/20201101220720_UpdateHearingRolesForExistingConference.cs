using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class UpdateHearingRolesForExistingConference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Update Participant Set HearingRole = 'Litigant in person'
                                    From Participant P
                                    Inner Join Conference C
                                    On P.ConferenceId = C.Id
                                    where C.State = 0 and C.ScheduledDateTime > getdate() and P.HearingRole like '%LIP'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
