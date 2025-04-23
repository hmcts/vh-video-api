using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class InstantMessageMap : IEntityTypeConfiguration<InstantMessage>
    {
        public void Configure(EntityTypeBuilder<InstantMessage> builder)
        {
            builder.ToTable(nameof(InstantMessage));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.From).IsRequired();
            builder.Property(x => x.To).IsRequired(false);
            builder.Property(x => x.MessageText).IsRequired();
            builder.Property(x => x.TimeStamp).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}
