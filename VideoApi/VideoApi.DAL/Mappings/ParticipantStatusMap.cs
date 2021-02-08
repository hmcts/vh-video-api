using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantStatusMap : IEntityTypeConfiguration<ParticipantStatus>
    {
        public void Configure(EntityTypeBuilder<ParticipantStatus> builder)
        {
            builder.ToTable(nameof(ParticipantStatus));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.ParticipantState);
            builder.Property(x => x.TimeStamp);
        }
    }
}