using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedOneToManyRelationshipToEndpointParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EndpointId",
                table: "Participant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_EndpointId",
                table: "Participant",
                column: "EndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Endpoint_EndpointId",
                table: "Participant",
                column: "EndpointId",
                principalTable: "Endpoint",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Endpoint_EndpointId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_EndpointId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "EndpointId",
                table: "Participant");
        }
    }
}
