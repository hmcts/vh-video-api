using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddVirtualCourt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VirtualCourtId",
                table: "Conference",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VirtualCourt",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdminUri = table.Column<string>(nullable: false),
                    JudgeUri = table.Column<string>(nullable: false),
                    ParticipantUri = table.Column<string>(nullable: false),
                    PexipNode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualCourt", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conference_VirtualCourtId",
                table: "Conference",
                column: "VirtualCourtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conference_VirtualCourt_VirtualCourtId",
                table: "Conference",
                column: "VirtualCourtId",
                principalTable: "VirtualCourt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conference_VirtualCourt_VirtualCourtId",
                table: "Conference");

            migrationBuilder.DropTable(
                name: "VirtualCourt");

            migrationBuilder.DropIndex(
                name: "IX_Conference_VirtualCourtId",
                table: "Conference");

            migrationBuilder.DropColumn(
                name: "VirtualCourtId",
                table: "Conference");
        }
    }
}
