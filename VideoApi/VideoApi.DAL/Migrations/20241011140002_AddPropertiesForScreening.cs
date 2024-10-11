using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesForScreening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceRole",
                table: "Endpoint",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ConferenceRoomType",
                table: "Conference",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConferenceRole",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "ConferenceRoomType",
                table: "Conference");
        }
    }
}
