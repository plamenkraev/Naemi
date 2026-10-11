using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class EkOblConfiguration : IEntityTypeConfiguration<EK_OBL>
{
    public void Configure(EntityTypeBuilder<EK_OBL> builder)
    {
        builder.ToTable("EK_OBL");
        builder.HasKey(o => o.ID);
        builder.Property(o => o.OBLAST).HasMaxLength(3);
        builder.Property(o => o.EKATTE).HasMaxLength(5);
        builder.Property(o => o.NAME).HasMaxLength(100);
    }
}
