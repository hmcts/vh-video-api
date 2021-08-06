using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class QuickLinkParticipantMap : IEntityTypeConfiguration<QuickLinkParticipant>
    {
        public void Configure(EntityTypeBuilder<QuickLinkParticipant> builder)
        {
            builder.HasOne(x => x.Token).WithOne(x => x.Participant);
        }
    }
}
