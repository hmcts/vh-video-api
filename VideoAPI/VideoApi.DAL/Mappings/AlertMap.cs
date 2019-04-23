using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class AlertMap : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable(nameof(Alert));
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Body);
            builder.Property(x => x.Type);
            builder.Property(x => x.Status);
            builder.Property(x => x.Created);
            builder.Property(x => x.Updated);
            builder.Property(x => x.UpdatedBy);
        }
    }
}