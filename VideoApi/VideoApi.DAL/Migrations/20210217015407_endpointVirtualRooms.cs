using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class endpointVirtualRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentVirtualRoomId",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomEndpoint",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndpointId = table.Column<Guid>(nullable: false),
                    RoomId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomEndpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomEndpoint_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_CurrentVirtualRoomId",
                table: "Endpoint",
                column: "CurrentVirtualRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEndpoint_RoomId",
                table: "RoomEndpoint",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Room_CurrentVirtualRoomId",
                table: "Endpoint",
                column: "CurrentVirtualRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Room_CurrentVirtualRoomId",
                table: "Endpoint");

            migrationBuilder.DropTable(
                name: "RoomEndpoint");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_CurrentVirtualRoomId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "CurrentVirtualRoomId",
                table: "Endpoint");
        }
    }
}
