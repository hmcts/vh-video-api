using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class RenameMonitoringToHeartbeat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable
            (
                "Monitoring",
                null,
                "Heartbeat"
            );
            
            migrationBuilder.RenameIndex
            (
                "IX_Monitoring_ConferenceId", 
                "IX_Heartbeat_ConferenceId",
                "Heartbeat"
            );
            
            migrationBuilder.RenameIndex
            (
                "IX_Monitoring_ParticipantId", 
                "IX_Heartbeat_ParticipantId",
                "Heartbeat"
            );
            
            migrationBuilder.RenameIndex
            (
                "PK_Monitoring", 
                "PK_Heartbeat",
                "Heartbeat"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable
            (
                "Heartbeat",
                null,
                "Monitoring"
            );
            
            migrationBuilder.RenameIndex
            (
                "IX_Heartbeat_ConferenceId",
                "IX_Monitoring_ConferenceId", 
                "Monitoring"
            );
            
            migrationBuilder.RenameIndex
            (
                "IX_Heartbeat_ParticipantId",
                "IX_Monitoring_ParticipantId", 
                "Monitoring"
            );

            migrationBuilder.RenameIndex
            (
                "PK_Heartbeat",
                "PK_Monitoring", 
                "Monitoring"
            );
        }
    }
}
