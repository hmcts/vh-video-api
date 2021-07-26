using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class MagicLinkParticipantMap : IEntityTypeConfiguration<MagicLinkParticipant>
    {
        public void Configure(EntityTypeBuilder<MagicLinkParticipant> builder)
        {
            builder.HasOne(x => x.Token).WithOne(x => x.Participant);
        }
    }
}
