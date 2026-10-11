using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class EkOblConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("EK_OBL");
        builder.HasKey(o => o.ID);
        builder.Property(o => o.OBLAST).HasMaxLength(3);
        builder.Property(o => o.Settlement).HasColumnName("EKATTE").HasMaxLength(5);
        builder.Property(o => o.NAME).HasMaxLength(100);
    }
}
