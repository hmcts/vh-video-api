using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class RoomEndpointMap : IEntityTypeConfiguration<RoomEndpoint>
    {
        public void Configure(EntityTypeBuilder<RoomEndpoint> builder)
        {
            builder.ToTable(nameof(RoomEndpoint));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.EndpointId);
        }
    }
}
