using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoApi.Domain;

namespace VideoApi.DAL.Mappings
{
    public class TestCallResultMap : IEntityTypeConfiguration<TestCallResult>
    {
        public void Configure(EntityTypeBuilder<TestCallResult> builder)
        {
            builder.ToTable(nameof(TestCallResult));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Passed);
            builder.Property(x => x.Score);
        }
    }
}