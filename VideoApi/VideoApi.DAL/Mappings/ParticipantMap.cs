using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class ParticipantMap : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.Property(x => x.ContactEmail);
            builder.Property(x => x.CurrentRoom);
            
            // Keep deleted properties as columns in the table
            builder.Property<string>("FirstName").HasColumnName("FirstName");
            builder.Property<string>("LastName").HasColumnName("LastName");
            builder.Property<string>("ContactTelephone").HasColumnName("ContactTelephone");
            builder.Property<string>("CaseTypeGroup").HasColumnName("CaseTypeGroup");
            builder.Property<string>("Representee").HasColumnName("Representee");
        }
    }
}
