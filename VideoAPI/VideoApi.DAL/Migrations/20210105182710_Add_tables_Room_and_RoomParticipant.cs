using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class Add_tables_Room_and_RoomParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConferenceId = table.Column<Guid>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
               name: "IX_Room_ConferenceId",
               table: "Room",
               column: "ConferenceId");

            migrationBuilder.CreateTable(
                name: "RoomParticipant",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<long>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomParticipant_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                       name: "FK_RoomParticipant_Participant_ParticipantId",
                       column: x => x.ParticipantId,
                       principalTable: "Participant",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipant_RoomId",
                table: "RoomParticipant",
                column: "RoomId");

            migrationBuilder.CreateIndex(
               name: "IX_RoomParticipant_ParticipantId",
               table: "RoomParticipant",
               column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomParticipant");

            migrationBuilder.DropTable(
                name: "Room");
        }
    }
}
