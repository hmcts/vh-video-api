using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RenameTimespanOnTestCallResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(name: "Timespan",
            table: "TestCallResult",
            newName: "Timestamp",
            schema: "dbo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "TestCallResult",
                newName: "Timespan",
                schema: "dbo");
        }
    }
}
