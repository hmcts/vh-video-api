using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class InterpreterRoomMap : IEntityTypeConfiguration<InterpreterRoom>
    {
        public void Configure(EntityTypeBuilder<InterpreterRoom> builder)
        {
            builder.Property(x => x.IngestUrl);
            builder.Property(x => x.PexipNode);
            builder.Property(x => x.ParticipantUri);
        }
    }
}
