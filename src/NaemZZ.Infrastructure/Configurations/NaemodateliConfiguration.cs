using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class NaemodateliConfiguration : IEntityTypeConfiguration<Landlord>
{
    public void Configure(EntityTypeBuilder<Landlord> builder)
    {
        builder.ToTable("NAEMODATELI");
        builder.HasKey(n => n.ID);

        builder.Property(n => n.NAME).HasMaxLength(100);
        builder.Property(n => n.SURNAME).HasMaxLength(100);
        builder.Property(n => n.LASTNAME).HasMaxLength(100);
        builder.Property(n => n.STREET).HasMaxLength(100);
        builder.Property(n => n.EGN).HasMaxLength(15);
        builder.Property(n => n.LK).HasMaxLength(15);
        builder.Property(n => n.MVR).HasMaxLength(25);
        builder.Property(n => n.Phone).HasMaxLength(20);

        builder.HasOne(n => n.Company)
            .WithMany(f => f.NAEMODATELIs)
            .HasForeignKey(n => n.FIRM_ID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Region)
            .WithMany(o => o.NAEMODATELIs)
            .HasForeignKey(n => n.OBLAST)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Municipality)
            .WithMany(o => o.NAEMODATELIs)
            .HasForeignKey(n => n.OBSTINA)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Settlement)
            .WithMany(e => e.NAEMODATELIs)
            .HasForeignKey(n => n.CITY)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(n => n.IMOTIs)
            .WithOne(i => i.Landlord)
            .HasForeignKey(i => i.LandlordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
