using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class EndpointParticipantMap : IEntityTypeConfiguration<EndpointParticipant>
    {
        public void Configure(EntityTypeBuilder<EndpointParticipant> builder)
        {
            builder.ToTable(nameof(EndpointParticipant));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.EndpointId).IsRequired();
            builder.Property(x => x.ParticipantUsername).IsRequired();
            builder.HasOne(x => x.Endpoint)
                .WithMany(x => x.EndpointParticipants)
                .HasForeignKey(x => x.EndpointId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
