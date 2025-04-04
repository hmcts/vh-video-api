using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Consts;
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
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.Username).HasConversion(v =>
                v.Replace(QuickLinkParticipantConst.Domain, "")
            , v => !v.Contains('@') ? $"{v}{QuickLinkParticipantConst.Domain}" : v);
            builder.Property(x => x.UserRole);
            builder.Property(x => x.CurrentRoom);

            builder.Property(x => x.HearingRole);

            builder.Property(x => x.TestCallResultId).IsRequired(false);
            builder.Property(x => x.CurrentConsultationRoomId).IsRequired(false);
            builder.Property(x => x.State).HasDefaultValue(ParticipantState.NotSignedIn);

            builder.HasMany<ParticipantStatus>("ParticipantStatuses").WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.LinkedParticipants).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId);

            builder.HasMany<RoomParticipant>(nameof(ParticipantBase.RoomParticipants)).WithOne(x => x.Participant)
                .HasForeignKey(x => x.ParticipantId);
            
            // Keep deleted properties as columns in the table
            builder.Property<string>("Name").HasColumnName("Name");
            
            builder.HasOne(participant => participant.Endpoint)
                .WithMany(endpoint => endpoint.ParticipantsLinked)
                .HasForeignKey(p => p.EndpointId)
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
