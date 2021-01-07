using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class RoomParticipantMap : IEntityTypeConfiguration<RoomParticipant>
    {
        public void Configure(EntityTypeBuilder<RoomParticipant> builder)
        {
            builder.ToTable(nameof(RoomParticipant));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoomId);
            builder.Property(x => x.ParticipantId);
            builder.Property(x => x.EnterTime);
        }
    }
}
