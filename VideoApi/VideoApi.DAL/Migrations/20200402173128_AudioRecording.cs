using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AudioRecording : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AudioRecordingRequired",
                table: "Conference",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IngestUrl",
                table: "Conference",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioRecordingRequired",
                table: "Conference");

            migrationBuilder.DropColumn(
                name: "IngestUrl",
                table: "Conference");
        }
    }
}
