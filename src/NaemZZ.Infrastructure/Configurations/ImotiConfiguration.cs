using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class ImotiConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("IMOTI");
        builder.HasKey(i => i.Id);
        builder.Property(x => x.Id).HasColumnName("imotiID");
        builder.Property(x => x.LandlordId).HasColumnName("nID");

        builder.Property(i => i.PLOST).HasColumnType("decimal(18,3)");
        builder.Property(i => i.TERRAIN).HasMaxLength(50);
        builder.Property(i => i.NOMER).HasMaxLength(10);
        builder.Property(i => i.LIMITS).HasMaxLength(100);

        builder.HasOne(i => i.PropertyCategory)
            .WithMany(c => c.IMOTIs)
            .HasForeignKey(i => i.CATEGORY)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Region)
            .WithMany(o => o.IMOTIs)
            .HasForeignKey(i => i.EKATTE_AREA)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Municipality)
            .WithMany(o => o.IMOTIs)
            .HasForeignKey(i => i.EKATTE_MUNICIPALITY)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Settlement)
            .WithMany(e => e.IMOTIs)
            .HasForeignKey(i => i.EKATTE_CITY)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
