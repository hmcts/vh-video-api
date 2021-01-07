using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class Remove_enterTime_field_table_roomParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterTime",
                table: "RoomParticipant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnterTime",
                table: "RoomParticipant",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
