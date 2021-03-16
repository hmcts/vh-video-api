using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantMap : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable(nameof(Participant));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.ParticipantRefId);
            builder.Property(x => x.Name);
            builder.Property(x => x.FirstName);
            builder.Property(x => x.LastName);
            builder.Property(x => x.ContactEmail);
            builder.Property(x => x.ContactTelephone);
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.Username);
            builder.Property(x => x.UserRole);
            builder.Property(x => x.HearingRole);
            builder.Property(x => x.CaseTypeGroup);
            builder.Property(x => x.Representee);
            builder.Property(x => x.CurrentRoom);

            builder.Property(x => x.TestCallResultId).IsRequired(false);
            builder.Property(x => x.CurrentConsultationRoomId).IsRequired(false);
            builder.Property(x => x.State).HasDefaultValue(ParticipantState.NotSignedIn);

            builder.HasMany<ParticipantStatus>("ParticipantStatuses").WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.LinkedParticipants).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId);
        }
    }
}
