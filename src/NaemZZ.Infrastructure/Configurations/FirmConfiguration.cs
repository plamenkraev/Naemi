using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class FirmConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("FIRM");
        builder.HasKey(f => f.Id);
        builder.Property(x => x.Id).HasColumnName("firmID");
        builder.Property(x => x.Name).HasColumnName("firmNAME");
        builder.Property(x => x.Address).HasColumnName("firmADDRESS");
        builder.Property(x => x.Representative).HasColumnName("firmMOL");
        builder.Property(x => x.RepresentativeEgn).HasColumnName("firmEGN");
        builder.Property(x => x.RepresentativeIdCard).HasColumnName("firmLK");
        builder.Property(x => x.RepresentativeIdCardDate).HasColumnName("firmLKDATE");
        builder.Property(x => x.RepresentativeIdCardIssuer).HasColumnName("firmMVR");
        builder.HasOne(f => f.ContractTemplate).WithMany(t => t.Firms).HasForeignKey(f => f.ContractTemplateId).OnDelete(DeleteBehavior.SetNull);

        builder.Property(f => f.Name).HasMaxLength(100);
        builder.Property(f => f.Address).HasMaxLength(100);
        builder.Property(f => f.BULSTAT).HasMaxLength(10);
        builder.Property(f => f.Representative).HasMaxLength(50);
        builder.Property(f => f.RepresentativeEgn).HasMaxLength(15);
        builder.Property(f => f.RepresentativeIdCard).HasMaxLength(15);
        builder.Property(f => f.RepresentativeIdCardIssuer).HasMaxLength(50);
    }
}
