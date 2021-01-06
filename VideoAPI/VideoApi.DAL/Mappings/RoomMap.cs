using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class RoomMap : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable(nameof(Room));

            builder.HasKey(x => x.Id);

            builder.HasMany<RoomParticipant>("RoomParticipants").WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ConferenceId);
            builder.Property(x => x.Label);
            builder.Property(x => x.Type);
            builder.Property(x => x.Status);

        }
    }
}
