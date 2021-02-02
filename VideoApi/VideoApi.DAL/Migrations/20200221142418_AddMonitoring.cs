using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddMonitoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monitoring",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConferenceId = table.Column<Guid>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: false),
                    OutgoingAudioPercentageLost = table.Column<decimal>(nullable: false),
                    OutgoingAudioPercentageLostRecent = table.Column<decimal>(nullable: false),
                    IncomingAudioPercentageLost = table.Column<decimal>(nullable: false),
                    IncomingAudioPercentageLostRecent = table.Column<decimal>(nullable: false),
                    OutgoingVideoPercentageLost = table.Column<decimal>(nullable: false),
                    OutgoingVideoPercentageLostRecent = table.Column<decimal>(nullable: false),
                    IncomingVideoPercentageLost = table.Column<decimal>(nullable: false),
                    IncomingVideoPercentageLostRecent = table.Column<decimal>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitoring", x => x.Id);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_Monitoring_ConferenceId",
                table: "Monitoring",
                column: "ConferenceId");
            
            migrationBuilder.CreateIndex(
                name: "IX_Monitoring_ParticipantId",
                table: "Monitoring",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monitoring");
        }
    }
}
