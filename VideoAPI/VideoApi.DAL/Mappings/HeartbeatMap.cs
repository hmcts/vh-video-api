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
            builder.Property(x => x.OutgoingAudioPercentageLost);
            builder.Property(x => x.OutgoingAudioPercentageLostRecent);
            builder.Property(x => x.IncomingAudioPercentageLost);
            builder.Property(x => x.IncomingAudioPercentageLostRecent);
            builder.Property(x => x.OutgoingVideoPercentageLost);
            builder.Property(x => x.OutgoingVideoPercentageLostRecent);
            builder.Property(x => x.IncomingVideoPercentageLost);
            builder.Property(x => x.IncomingVideoPercentageLostRecent);
            builder.Property(x => x.BrowserName);
            builder.Property(x => x.BrowserVersion);
            builder.Property(x => x.Timestamp);
        }
    }
}
