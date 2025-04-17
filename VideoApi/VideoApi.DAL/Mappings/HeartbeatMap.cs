using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using DataTypes = VideoApi.DAL.Constants.DataTypes;

namespace VideoApi.DAL.Mappings
{
    public class HeartbeatMap : IEntityTypeConfiguration<Heartbeat>
    {
        public void Configure(EntityTypeBuilder<Heartbeat> builder)
        {
            builder.ToTable(nameof(Heartbeat));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConferenceId);
            builder.Property(x => x.ParticipantId);
            builder.Property(x => x.OutgoingAudioPercentageLost).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.OutgoingAudioPercentageLostRecent).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.IncomingAudioPercentageLost).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.IncomingAudioPercentageLostRecent).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.OutgoingVideoPercentageLost).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.OutgoingVideoPercentageLostRecent).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.IncomingVideoPercentageLost).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.IncomingVideoPercentageLostRecent).HasColumnType(DataTypes.Decimal);
            builder.Property(x => x.BrowserName);
            builder.Property(x => x.BrowserVersion);
            builder.Property(x => x.OperatingSystem);
            builder.Property(x => x.OperatingSystemVersion);
            builder.Property(x => x.OutgoingAudioPacketsLost).HasColumnType(DataTypes.Int);
            builder.Property(x => x.OutgoingAudioBitrate);
            builder.Property(x => x.OutgoingAudioCodec);
            builder.Property(x => x.OutgoingAudioPacketSent).HasColumnType(DataTypes.Int);
            builder.Property(x => x.OutgoingVideoPacketSent).HasColumnType(DataTypes.Int);
            builder.Property(x => x.OutgoingVideoPacketsLost).HasColumnType(DataTypes.Int);
            builder.Property(x => x.OutgoingVideoFramerate).HasColumnType(DataTypes.Int);
            builder.Property(x => x.OutgoingVideoBitrate);
            builder.Property(x => x.OutgoingVideoCodec);
            builder.Property(x => x.OutgoingVideoResolution);
            builder.Property(x => x.IncomingAudioBitrate);
            builder.Property(x => x.IncomingAudioCodec);
            builder.Property(x => x.IncomingAudioPacketReceived).HasColumnType(DataTypes.Int);
            builder.Property(x => x.IncomingAudioPacketsLost).HasColumnType(DataTypes.Int);
            builder.Property(x => x.IncomingVideoBitrate);
            builder.Property(x => x.IncomingVideoCodec);
            builder.Property(x => x.IncomingVideoResolution);
            builder.Property(x => x.IncomingVideoPacketReceived).HasColumnType(DataTypes.Int);
            builder.Property(x => x.IncomingVideoPacketsLost).HasColumnType(DataTypes.Int);
            builder.Property(x => x.Timestamp).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}
