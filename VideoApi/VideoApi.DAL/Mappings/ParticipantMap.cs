using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantMap : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.Property(x => x.FirstName);
            builder.Property(x => x.LastName);
            builder.Property(x => x.ContactEmail);
            builder.Property(x => x.ContactTelephone);
            builder.Property(x => x.CaseTypeGroup);
            builder.Property(x => x.Representee);
            builder.Property(x => x.CurrentRoom);
        }
    }
}
