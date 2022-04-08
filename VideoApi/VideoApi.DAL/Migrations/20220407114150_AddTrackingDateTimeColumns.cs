using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddTrackingDateTimeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "TestCallResult",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RoomParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RoomParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RoomEndpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RoomEndpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Room",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Participant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Participant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LinkedParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LinkedParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conference",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "TestCallResult");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RoomParticipant");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RoomParticipant");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RoomEndpoint");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RoomEndpoint");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LinkedParticipant");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LinkedParticipant");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conference");
        }
    }
}
