using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class MessageMap : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable(nameof(Message));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.From).IsRequired();
            builder.Property(x => x.To).IsRequired(false);
            builder.Property(x => x.MessageText).IsRequired();
            builder.Property(x => x.TimeStamp);
        }
    }
}
