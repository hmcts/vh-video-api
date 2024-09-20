using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Mappings;

public class TelephoneParticipantMap : IEntityTypeConfiguration<TelephoneParticipant>
{
    public void Configure(EntityTypeBuilder<TelephoneParticipant> builder)
    {
        builder.ToTable(nameof(TelephoneParticipant));
            
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TelephoneNumber).IsRequired();
        builder.Property(x => x.State).IsRequired().HasDefaultValue(TelephoneState.Connected);
        builder.Property(x => x.CurrentRoom).IsRequired().HasDefaultValue(RoomType.WaitingRoom);
    }
}
