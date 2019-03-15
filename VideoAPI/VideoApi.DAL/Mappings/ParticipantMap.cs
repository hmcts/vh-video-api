using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

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
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.Username);
            builder.Property(x => x.UserRole);
            builder.Property(x => x.CaseTypeGroup);

            builder.HasMany<ParticipantStatus>("ParticipantStatuses").WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}