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
            builder.Property(x => x.Timestamp);
        }
    }
}
