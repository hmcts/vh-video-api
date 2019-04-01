using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

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
            builder.Property(x => x.ScheduledDuration);
            builder.Property<ConferenceState>("State");

            builder.HasMany<Participant>("Participants").WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany<ConferenceStatus>("ConferenceStatuses").WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.AdminUri).HasColumnName("AdminUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.JudgeUri).HasColumnName("JudgeUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.ParticipantUri).HasColumnName("ParticipantUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.PexipNode).HasColumnName("PexipNode");
        }
    }
}