using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class FirmConfiguration : IEntityTypeConfiguration<FIRM>
{
    public void Configure(EntityTypeBuilder<FIRM> builder)
    {
        builder.ToTable("FIRM");
        builder.HasKey(f => f.firmID);
        builder.HasOne(f => f.ContractTemplate).WithMany(t => t.Firms).HasForeignKey(f => f.ContractTemplateId).OnDelete(DeleteBehavior.SetNull);

        builder.Property(f => f.firmNAME).HasMaxLength(100);
        builder.Property(f => f.firmADDRESS).HasMaxLength(100);
        builder.Property(f => f.BULSTAT).HasMaxLength(10);
        builder.Property(f => f.firmMOL).HasMaxLength(50);
        builder.Property(f => f.firmEGN).HasMaxLength(15);
        builder.Property(f => f.firmLK).HasMaxLength(15);
        builder.Property(f => f.firmMVR).HasMaxLength(50);
    }
}
