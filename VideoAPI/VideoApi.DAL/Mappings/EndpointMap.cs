using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class EndpointMap : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.ToTable(nameof(Endpoint));
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DisplayName).IsRequired();
            builder.Property(x => x.SipAddress).IsRequired();
            builder.HasIndex(x => x.SipAddress).IsUnique();
            builder.Property(x => x.Pin).IsRequired();
            builder.Property(x => x.State).IsRequired();
            builder.Property(x => x.DefenceAdvocate).HasMaxLength(450);
        }
    }
}
