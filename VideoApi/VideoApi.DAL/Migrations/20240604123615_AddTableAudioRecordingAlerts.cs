using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAudioRecordingAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudioRecordingAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConferenceId = table.Column<Guid>(nullable: false),
                    CaseNumber = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdateAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioRecordingAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioRecordingAlerts_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordingAlerts_ConferenceId",
                table: "AudioRecordingAlerts",
                column: "ConferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioRecordingAlerts");
        }
    }
}
