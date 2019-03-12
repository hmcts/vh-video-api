using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class EventMap : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(nameof(Event));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConferenceId);
            builder.Property(x => x.ExternalEventId);
            builder.Property(x => x.EventType);
            builder.Property(x => x.ExternalTimestamp);
            builder.Property(x => x.ParticipantId);
            builder.Property(x => x.TransferredFrom);
            builder.Property(x => x.TransferredTo);
            builder.Property(x => x.Reason);
        }
    }
}