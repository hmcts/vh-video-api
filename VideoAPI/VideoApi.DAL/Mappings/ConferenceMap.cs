using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ConferenceMap : IEntityTypeConfiguration<Conference>
    {
        public void Configure(EntityTypeBuilder<Conference> builder)
        {
            builder.ToTable(nameof(Conference));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.HearingRefId);
            builder.Property(x => x.CaseType);
            builder.Property(x => x.ScheduledDateTime);
            builder.Property(x => x.CaseNumber);
            builder.Property(x => x.CaseName);

            builder.HasMany<Participant>("Participants");
            builder.HasMany<ConferenceStatus>("ConferenceStatuses");
        }
    }
}