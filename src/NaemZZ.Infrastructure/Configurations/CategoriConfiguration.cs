using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;

namespace NaemZZ.Infrastructure.Configurations;

public class CategoriConfiguration : IEntityTypeConfiguration<PropertyCategory>
{
    public void Configure(EntityTypeBuilder<PropertyCategory> builder)
    {
        builder.ToTable("CATEGORI");
        builder.HasKey(c => c.ID);
        builder.Property(c => c.NAME).HasMaxLength(3);
    }
}
