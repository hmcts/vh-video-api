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
            builder.Property(x => x.ClosedDateTime);
            builder.Property<ConferenceState>("State");
            builder.Property(x => x.AudioRecordingRequired);
            builder.Property(x => x.IngestUrl);
            builder.Property(x => x.ActualStartTime);
            builder.Property(x => x.CreatedDateTime);

            builder.HasMany<Participant>("Participants").WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Endpoints).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany<ConferenceStatus>("ConferenceStatuses").WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.InstantMessageHistory).WithOne().OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.AdminUri).HasColumnName("AdminUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.JudgeUri).HasColumnName("JudgeUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.ParticipantUri).HasColumnName("ParticipantUri");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.PexipNode).HasColumnName("PexipNode");
            builder.OwnsOne<MeetingRoom>("MeetingRoom").Property(x => x.TelephoneConferenceId).HasColumnName("TelephoneConferenceId");
        }
    }
}
