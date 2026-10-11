using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class EkObstConfiguration : IEntityTypeConfiguration<Municipality>
{
    public void Configure(EntityTypeBuilder<Municipality> builder)
    {
        builder.ToTable("EK_OBST");
        builder.HasKey(o => o.ID);
        builder.Property(o => o.Settlement).HasColumnName("EKATTE").HasMaxLength(5);
        builder.Property(o => o.NAME).HasMaxLength(100);

        builder.HasOne(o => o.Region)
            .WithMany(r => r.EK_OBSTs)
            .HasForeignKey(o => o.OBL_ID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
