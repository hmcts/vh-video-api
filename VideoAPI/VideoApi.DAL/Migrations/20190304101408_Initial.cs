using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conference",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HearingRefId = table.Column<Guid>(nullable: false),
                    CaseType = table.Column<string>(nullable: true),
                    ScheduledDateTime = table.Column<DateTime>(nullable: false),
                    CaseNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExternalEventId = table.Column<string>(nullable: true),
                    EventType = table.Column<int>(nullable: false),
                    ExternalTimestamp = table.Column<DateTime>(nullable: false),
                    ParticipantId = table.Column<string>(nullable: true),
                    TransferredFrom = table.Column<int>(nullable: true),
                    TransferredTo = table.Column<int>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConferenceStatus",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConferenceState = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    ConferenceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConferenceStatus_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParticipantRefId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    HearingRole = table.Column<string>(nullable: true),
                    CaseTypeGroup = table.Column<string>(nullable: true),
                    ConferenceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participant_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantStatus",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParticipantState = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    ParticipantId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantStatus_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceStatus_ConferenceId",
                table: "ConferenceStatus",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_ConferenceId",
                table: "Participant",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantStatus_ParticipantId",
                table: "ParticipantStatus",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConferenceStatus");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "ParticipantStatus");

            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "Conference");
        }
    }
}
