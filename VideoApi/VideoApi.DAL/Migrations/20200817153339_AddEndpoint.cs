using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddEndpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Endpoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    SipAddress = table.Column<string>(nullable: false),
                    Pin = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ConferenceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endpoint_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_ConferenceId",
                table: "Endpoint",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_SipAddress",
                table: "Endpoint",
                column: "SipAddress",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Endpoint");
        }
    }
}
