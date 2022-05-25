using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApi.DAL.Migrations
{
    public partial class AddBitrateCodecFrameRateColumnsToHeartbeatTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutgoingAudioPacketSent",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutgoingAudioPacketsLost",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutgoingVideoPacketSent",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutgoingVideoPacketsLost",
                table: "Heartbeat",
                nullable: true);
            
            migrationBuilder.AddColumn<int>(
                name: "IncomingAudioPacketReceived",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomingAudioPacketsLost",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomingVideoPacketReceived",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomingVideoPacketsLost",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutgoingVideoFramerate",
                table: "Heartbeat",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutgoingVideoBitrate",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutgoingVideoCodec",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutgoingVideoResolution",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutgoingAudioBitrate",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutgoingAudioCodec",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength:50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingAudioBitrate",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingAudioCodec",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingVideoBitrate",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingVideoCodec",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingVideoResolution",
                table: "Heartbeat",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "OutgoingAudioPacketSent",
            table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingAudioPacketsLost",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoPacketSent",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoPacketsLost",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingAudioPacketReceived",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingAudioPacketsLost",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingVideoPacketReceived",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingVideoPacketsLost",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoFramerate",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoBitrate",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoCodec",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingVideoResolution",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingAudioBitrate",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "OutgoingAudioCodec",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingAudioBitrate",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingAudioCodec",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingVideoBitrate",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingVideoCodec",
                table: "Heartbeat");

            migrationBuilder.DropColumn(
                name: "IncomingVideoResolution",
                table: "Heartbeat");

            
        }
    }
}
