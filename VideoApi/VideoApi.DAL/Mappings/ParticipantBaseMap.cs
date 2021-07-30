using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantBaseMap : IEntityTypeConfiguration<ParticipantBase>
    {
        public void Configure(EntityTypeBuilder<ParticipantBase> builder)
        {
            builder.ToTable(nameof(Participant));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ParticipantRefId);
            builder.Property(x => x.Name);
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.Username);
            builder.Property(x => x.UserRole);
            builder.Property(x => x.CurrentRoom);

            builder.Property(x => x.TestCallResultId).IsRequired(false);
            builder.Property(x => x.CurrentConsultationRoomId).IsRequired(false);
            builder.Property(x => x.State).HasDefaultValue(ParticipantState.NotSignedIn);

            builder.HasMany<ParticipantStatus>("ParticipantStatuses").WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.LinkedParticipants).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId);

            builder.HasMany<RoomParticipant>(nameof(ParticipantBase.RoomParticipants)).WithOne(x => x.Participant)
                .HasForeignKey(x => x.ParticipantId);
        }
    }
}
