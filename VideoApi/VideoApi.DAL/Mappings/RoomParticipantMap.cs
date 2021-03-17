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
            builder.Property(x => x.ParticipantId);
            builder.HasOne(x => x.Participant).WithMany(x => x.RoomParticipants).HasForeignKey(x => x.ParticipantId);
            builder.HasOne(x => x.Room).WithMany(x => x.RoomParticipants).HasForeignKey(x => x.RoomId);
        }
    }
}
