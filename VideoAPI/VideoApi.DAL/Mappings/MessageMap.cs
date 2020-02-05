using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class MessageMap : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable(nameof(Message));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.From);
            builder.Property(x => x.To);
            builder.Property(x => x.MessageText);
            builder.Property(x => x.TimeStamp);
        }
    }
}
