using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ConferenceStatusMap : IEntityTypeConfiguration<ConferenceStatus>
    {
        public void Configure(EntityTypeBuilder<ConferenceStatus> builder)
        {
            builder.ToTable(nameof(ConferenceStatus));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.ConferenceState);
            builder.Property(x => x.TimeStamp);
        }
    }
}
