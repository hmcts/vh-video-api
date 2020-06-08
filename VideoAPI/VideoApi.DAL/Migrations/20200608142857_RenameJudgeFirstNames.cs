using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RenameJudgeFirstNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Birmingham', 'Birmingham CFJC') FROM Participant where Name LIKE '%Birmingham Courtroom%'");
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Manchester', 'Manchester CFJC') FROM Participant where Name LIKE '%Manchester Courtroom%'");
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Taylorhouse', 'Taylor House') FROM Participant where Name LIKE '%Taylorhouse Courtroom%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Birmingham CFJC', 'Birmingham') FROM Participant where Name LIKE '%Birmingham CFJC Courtroom%'");
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Manchester CFJC', 'Manchester') FROM Participant where Name LIKE '%Manchester CFJC Courtroom%'");
            migrationBuilder.Sql("UPDATE Participant set Name = REPLACE(Name, 'Taylor House', 'Taylorhouse') FROM Participant where Name LIKE '%Taylor House Courtroom%'");
        }
    }
}
