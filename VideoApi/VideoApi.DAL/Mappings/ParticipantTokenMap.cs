using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantTokenMap : IEntityTypeConfiguration<ParticipantToken>
    {
        public void Configure(EntityTypeBuilder<ParticipantToken> builder)
        {
            builder.ToTable(nameof(ParticipantToken));
            builder.Property(x => x.Jwt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired()
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}
