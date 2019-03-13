using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class VirtualCourtMap : IEntityTypeConfiguration<VirtualCourt>
    {
        public void Configure(EntityTypeBuilder<VirtualCourt> builder)
        {
            builder.ToTable(nameof(VirtualCourt));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.AdminUri).IsRequired();
            builder.Property(x => x.JudgeUri).IsRequired();
            builder.Property(x => x.ParticipantUri).IsRequired();
            builder.Property(x => x.PexipNode).IsRequired();
        }
    }
}