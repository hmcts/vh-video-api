using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddMonitoring_browser_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Monitoring",
                table: "Monitoring");

            migrationBuilder.AddColumn<string>(
                name: "BrowserName",
                table: "Monitoring",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrowserVersion",
                table: "Monitoring",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Monitoring",
                table: "Monitoring",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Monitoring",
                table: "Monitoring");

            migrationBuilder.DropColumn(
                name: "BrowserName",
                table: "Monitoring");

            migrationBuilder.DropColumn(
                name: "BrowserVersion",
                table: "Monitoring");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Monitoring",
                table: "Monitoring",
                column: "Id");
        }
    }
}
