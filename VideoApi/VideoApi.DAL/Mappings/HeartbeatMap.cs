using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

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
            builder.Property(x => x.OutgoingAudioPercentageLost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.OutgoingAudioPercentageLostRecent).HasColumnType("decimal(18,2)");
            builder.Property(x => x.IncomingAudioPercentageLost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.IncomingAudioPercentageLostRecent).HasColumnType("decimal(18,2)");
            builder.Property(x => x.OutgoingVideoPercentageLost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.OutgoingVideoPercentageLostRecent).HasColumnType("decimal(18,2)");
            builder.Property(x => x.IncomingVideoPercentageLost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.IncomingVideoPercentageLostRecent).HasColumnType("decimal(18,2)");
            builder.Property(x => x.BrowserName);
            builder.Property(x => x.BrowserVersion);
            builder.Property(x => x.OperatingSystem);
            builder.Property(x => x.OperatingSystemVersion);
            builder.Property(x => x.OutgoingAudioPacketsLost).HasColumnType("int");
            builder.Property(x => x.OutgoingAudioBitrate);
            builder.Property(x => x.OutgoingAudioCodec);
            builder.Property(x => x.OutgoingAudioPacketSent).HasColumnType("int");
            builder.Property(x => x.OutgoingVideoPacketSent).HasColumnType("int");
            builder.Property(x => x.OutgoingVideoPacketsLost).HasColumnType("int");
            builder.Property(x => x.OutgoingVideoFramerate).HasColumnType("int");
            builder.Property(x => x.OutgoingVideoBitrate);
            builder.Property(x => x.OutgoingVideoCodec);
            builder.Property(x => x.OutgoingVideoResolution);
            builder.Property(x => x.IncomingAudioBitrate);
            builder.Property(x => x.IncomingAudioCodec);
            builder.Property(x => x.IncomingAudioPacketReceived).HasColumnType("int");
            builder.Property(x => x.IncomingAudioPacketsLost).HasColumnType("int");
            builder.Property(x => x.IncomingVideoBitrate);
            builder.Property(x => x.IncomingVideoCodec);
            builder.Property(x => x.IncomingVideoResolution);
            builder.Property(x => x.IncomingVideoPacketReceived).HasColumnType("int");
            builder.Property(x => x.IncomingVideoPacketsLost).HasColumnType("int");
            builder.Property(x => x.Timestamp);
        }
    }
}
