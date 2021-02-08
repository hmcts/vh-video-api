using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

public class LinkedParticipantMap : IEntityTypeConfiguration<LinkedParticipant>
{
    public void Configure(EntityTypeBuilder<LinkedParticipant> builder)
    {
        builder.ToTable(nameof(LinkedParticipant));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.ParticipantId).IsRequired();
        builder.Property(x => x.LinkedId).IsRequired();
        builder.Property(x => x.Type);
        builder.HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(x => x.Linked)
            .WithMany()
            .HasForeignKey(x => x.LinkedId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
