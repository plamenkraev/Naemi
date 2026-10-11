using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class EkObstConfiguration : IEntityTypeConfiguration<EK_OBST>
{
    public void Configure(EntityTypeBuilder<EK_OBST> builder)
    {
        builder.ToTable("EK_OBST");
        builder.HasKey(o => o.ID);
        builder.Property(o => o.EKATTE).HasMaxLength(5);
        builder.Property(o => o.NAME).HasMaxLength(100);

        builder.HasOne(o => o.EK_OBL)
            .WithMany(r => r.EK_OBSTs)
            .HasForeignKey(o => o.OBL_ID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
