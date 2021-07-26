using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantTokenMap : IEntityTypeConfiguration<ParticipantToken>
    {
        public void Configure(EntityTypeBuilder<ParticipantToken> builder)
        {
            builder.Property(x => x.Jwt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
        }
    }
}
