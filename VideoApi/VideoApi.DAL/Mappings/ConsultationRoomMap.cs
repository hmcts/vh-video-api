using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ConsultationRoomMap : IEntityTypeConfiguration<ConsultationRoom>
    {
        public void Configure(EntityTypeBuilder<ConsultationRoom> builder)
        {
            // Method intentionally left empty.
        }
    }
}
