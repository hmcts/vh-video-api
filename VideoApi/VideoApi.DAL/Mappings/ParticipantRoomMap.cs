using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantRoomMap : IEntityTypeConfiguration<ParticipantRoom>
    {
        public void Configure(EntityTypeBuilder<ParticipantRoom> builder)
        {
            builder.Property(x => x.IngestUrl);
            builder.Property(x => x.PexipNode);
            builder.Property(x => x.ParticipantUri);
        }
    }
}
