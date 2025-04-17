using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class TaskMap : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable(nameof(Task));
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ConferenceId);
            builder.HasIndex(x => x.ConferenceId);
            builder.Property(x => x.OriginId);
            builder.Property(x => x.Body);
            builder.Property(x => x.Type);
            builder.Property(x => x.Status);
            builder.Property(x => x.Created).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.Updated).HasConversion(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);
            builder.Property(x => x.UpdatedBy);
        }
    }
}
