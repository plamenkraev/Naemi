using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class EkatteConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("EKATTE");
        builder.HasKey(e => e.ID);

        // Legacy column name is "Settlement" (same as the table); the C# property was renamed
        // to EKATTE_CODE to avoid a class/member name collision - map it back explicitly.
        builder.Property(e => e.EKATTE_CODE).HasColumnName("EKATTE").HasMaxLength(5);
        builder.Property(e => e.T_V_M).HasMaxLength(4);
        builder.Property(e => e.NAME).HasMaxLength(100);

        builder.HasOne(e => e.Municipality)
            .WithMany(o => o.EKATTEs)
            .HasForeignKey(e => e.OBST_ID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
