using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class ImotiConfiguration : IEntityTypeConfiguration<IMOTI>
{
    public void Configure(EntityTypeBuilder<IMOTI> builder)
    {
        builder.ToTable("IMOTI");
        builder.HasKey(i => i.imotiID);

        builder.Property(i => i.PLOST).HasColumnType("decimal(18,3)");
        builder.Property(i => i.TERRAIN).HasMaxLength(50);
        builder.Property(i => i.NOMER).HasMaxLength(10);
        builder.Property(i => i.LIMITS).HasMaxLength(100);

        builder.HasOne(i => i.CATEGORI)
            .WithMany(c => c.IMOTIs)
            .HasForeignKey(i => i.CATEGORY)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.EK_OBL)
            .WithMany(o => o.IMOTIs)
            .HasForeignKey(i => i.EKATTE_AREA)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.EK_OBST)
            .WithMany(o => o.IMOTIs)
            .HasForeignKey(i => i.EKATTE_MUNICIPALITY)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.EKATTE)
            .WithMany(e => e.IMOTIs)
            .HasForeignKey(i => i.EKATTE_CITY)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
