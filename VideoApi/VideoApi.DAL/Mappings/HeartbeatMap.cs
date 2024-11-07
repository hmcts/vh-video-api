using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class HeartbeatMap : IEntityTypeConfiguration<Heartbeat>
    {
        private static class Constants
        {
            public const string Decimal = "decimal(18,2)";
            public const string Int = "int";
        }
        
        public void Configure(EntityTypeBuilder<Heartbeat> builder)
        {
            builder.ToTable(nameof(Heartbeat));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConferenceId);
            builder.Property(x => x.ParticipantId);
            builder.Property(x => x.OutgoingAudioPercentageLost).HasColumnType(Constants.Decimal);
            builder.Property(x => x.OutgoingAudioPercentageLostRecent).HasColumnType(Constants.Decimal);
            builder.Property(x => x.IncomingAudioPercentageLost).HasColumnType(Constants.Decimal);
            builder.Property(x => x.IncomingAudioPercentageLostRecent).HasColumnType(Constants.Decimal);
            builder.Property(x => x.OutgoingVideoPercentageLost).HasColumnType(Constants.Decimal);
            builder.Property(x => x.OutgoingVideoPercentageLostRecent).HasColumnType(Constants.Decimal);
            builder.Property(x => x.IncomingVideoPercentageLost).HasColumnType(Constants.Decimal);
            builder.Property(x => x.IncomingVideoPercentageLostRecent).HasColumnType(Constants.Decimal);
            builder.Property(x => x.BrowserName);
            builder.Property(x => x.BrowserVersion);
            builder.Property(x => x.OperatingSystem);
            builder.Property(x => x.OperatingSystemVersion);
            builder.Property(x => x.OutgoingAudioPacketsLost).HasColumnType(Constants.Int);
            builder.Property(x => x.OutgoingAudioBitrate);
            builder.Property(x => x.OutgoingAudioCodec);
            builder.Property(x => x.OutgoingAudioPacketSent).HasColumnType(Constants.Int);
            builder.Property(x => x.OutgoingVideoPacketSent).HasColumnType(Constants.Int);
            builder.Property(x => x.OutgoingVideoPacketsLost).HasColumnType(Constants.Int);
            builder.Property(x => x.OutgoingVideoFramerate).HasColumnType(Constants.Int);
            builder.Property(x => x.OutgoingVideoBitrate);
            builder.Property(x => x.OutgoingVideoCodec);
            builder.Property(x => x.OutgoingVideoResolution);
            builder.Property(x => x.IncomingAudioBitrate);
            builder.Property(x => x.IncomingAudioCodec);
            builder.Property(x => x.IncomingAudioPacketReceived).HasColumnType(Constants.Int);
            builder.Property(x => x.IncomingAudioPacketsLost).HasColumnType(Constants.Int);
            builder.Property(x => x.IncomingVideoBitrate);
            builder.Property(x => x.IncomingVideoCodec);
            builder.Property(x => x.IncomingVideoResolution);
            builder.Property(x => x.IncomingVideoPacketReceived).HasColumnType(Constants.Int);
            builder.Property(x => x.IncomingVideoPacketsLost).HasColumnType(Constants.Int);
            builder.Property(x => x.Timestamp);
        }
    }
}
